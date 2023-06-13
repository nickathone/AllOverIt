using AllOverIt.Assertion;
using AllOverIt.Async;
using AllOverIt.Pipes.Events;
using AllOverIt.Pipes.Exceptions;
using AllOverIt.Pipes.Serialization;
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Connection
{
    internal abstract class PipeConnection<TMessage> : IPipeConnection<TMessage>
    {
        private readonly IMessageSerializer<TMessage> _serializer;

        private CancellationTokenSource _cancellationTokenSource;
        private BackgroundTask _backgroundReader;
        private PipeReaderWriter _pipeReaderWriter;


        // A NamedPipeClientStream or NamedPipeServerStream
        protected PipeStream _pipeStream;


        /// <inheritdoc />
        public string PipeName { get; }

        /// <inheritdoc />
        public bool IsConnected => _pipeStream?.IsConnected ?? false;

        internal PipeConnection(PipeStream stream, string pipeName, IMessageSerializer<TMessage> serializer)
        {
            _pipeStream = stream.WhenNotNull(nameof(stream));               // Assume ownership of this stream
            PipeName = pipeName.WhenNotNullOrEmpty(nameof(pipeName));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }

        /// <inheritdoc />
        public void Connect()           // Cannot be re-connectd once disconnected
        {
            // TODO: Custom exception
            Throw<InvalidOperationException>.WhenNotNull(_backgroundReader, "The connection is already open.");

            _cancellationTokenSource = new CancellationTokenSource();

            _pipeReaderWriter = new PipeReaderWriter(_pipeStream, true);

            _backgroundReader = new BackgroundTask(async cancellationToken =>
            {
                while (!cancellationToken.IsCancellationRequested && IsConnected)
                {
                    try
                    {
                        var bytes = await _pipeReaderWriter
                            .ReadAsync(cancellationToken)
                            .ConfigureAwait(false);

                        if (!bytes.Any() || !IsConnected)
                        {
                            break;
                        }

                        var @object = _serializer.Deserialize(bytes);

                        DoOnMessageReceived(@object);
                    }
                    catch (IOException)
                    {
                        // PipeStreamReader will throw IOException if an expected byte count is not received.
                        // This can occur if the connection is killed during communication. Fall through so
                        // the connection is treated as disconnected.
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }

                DoOnDisconnected();
            },
            edi =>
            {
                DoOnException(edi.SourceException);
                return false;
            },
            _cancellationTokenSource.Token);
        }

        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            if (_cancellationTokenSource is not null)
            {
                _cancellationTokenSource.Cancel();

                await _backgroundReader.DisposeAsync().ConfigureAwait(false);
                _backgroundReader = null;

                await _pipeReaderWriter.DisposeAsync().ConfigureAwait(false);
                _pipeReaderWriter = null;

                await _pipeStream.DisposeAsync().ConfigureAwait(false);
                _pipeStream = null;

                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        /// <inheritdoc />
        public async Task WriteAsync(TMessage value, CancellationToken cancellationToken = default)
        {
            if (!IsConnected || !_pipeReaderWriter.CanWrite)
            {
                throw new NotConnectedException("Connection is not connected or writable.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            var bytes = _serializer.Serialize(value);

            await _pipeReaderWriter.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync().ConfigureAwait(false);
        }

        protected abstract void DoOnDisconnected();
        protected abstract void DoOnMessageReceived(TMessage message);
        protected abstract void DoOnException(Exception exception);
    }
}