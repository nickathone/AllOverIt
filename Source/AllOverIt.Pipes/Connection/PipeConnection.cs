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
    public sealed class PipeConnection<TType> : IPipeConnection<TType>, IAsyncDisposable
    {
        private readonly IMessageSerializer<TType> _serializer;

        // A NamedPipeClientStream or NamedPipeServerStream
        private PipeStream _pipeStream;

        private CancellationTokenSource _cancellationTokenSource;
        private BackgroundTask _backgroundReader;
        private PipeReaderWriter _pipeReaderWriter;

        /// <inheritdoc />
        public event EventHandler<ConnectionMessageEventArgs<TType>> OnMessageReceived;

        /// <inheritdoc />
        public event EventHandler<ConnectionEventArgs<TType>> OnDisconnected;

        /// <inheritdoc />
        public event EventHandler<ConnectionExceptionEventArgs<TType>> OnException;

        /// <inheritdoc />
        public string PipeName { get; }

        /// <inheritdoc />
        public string ServerName { get; }

        /// <inheritdoc />
        public bool IsConnected => _pipeStream?.IsConnected ?? false;



        internal PipeConnection(PipeStream stream, string pipeName, IMessageSerializer<TType> serializer)
        {
            _pipeStream = stream.WhenNotNull(nameof(stream));               // Assume ownership of this stream
            PipeName = pipeName.WhenNotNullOrEmpty(nameof(pipeName));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }

        internal PipeConnection(PipeStream stream, string pipeName, IMessageSerializer<TType> serializer, string serverName)
            : this(stream, pipeName, serializer)
        {
            ServerName = serverName.WhenNotNullOrEmpty(nameof(serverName));
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
                DoOnExceptionOccurred(edi.SourceException);
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
        public async Task WriteAsync(TType value, CancellationToken cancellationToken = default)
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
        public string GetImpersonationUserName()
        {
            if (_pipeStream is not NamedPipeServerStream serverStream)
            {
                throw new PipeConnectionException($"The pipe stream is not a {nameof(NamedPipeServerStream)}.");
            }

            // IOException will be raised of the pipe connection has been broken or
            // the user name is longer than 19 characters.
            return serverStream.GetImpersonationUserName();
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync().ConfigureAwait(false);
        }

        private void DoOnDisconnected()
        {
            var onDisconnected = OnDisconnected;

            if (onDisconnected is not null)
            {
                var args = new ConnectionEventArgs<TType>(this);

                onDisconnected.Invoke(this, args);
            }
        }

        private void DoOnMessageReceived(TType message)
        {
            var onMessageReceived = OnMessageReceived;

            if (onMessageReceived is not null)
            {
                var args = new ConnectionMessageEventArgs<TType>(this, message);

                onMessageReceived.Invoke(this, args);
            }
        }

        private void DoOnExceptionOccurred(Exception exception)
        {
            var onException = OnException;

            if (onException is not null)
            {
                var args = new ConnectionExceptionEventArgs<TType>(this, exception);

                onException.Invoke(this, args);
            }
        }
    }
}