using AllOverIt.Assertion;
using AllOverIt.Async;
using AllOverIt.Pipes.Events;
using AllOverIt.Pipes.Serialization;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Connection
{
    // This connection is not re-usable (connect / disconnect) since the decorated stream is disposed of.

    public sealed class PipeConnection<TType> : IPipeConnection<TType>, IAsyncDisposable
    {
        private readonly IMessageSerializer<TType> _serializer;

        // Decorated pipe stream (NamedPipeClientStream or NamedPipeServerStream).
        private PipeStream _pipeStream;

        private CancellationTokenSource _cancellationTokenSource;
        private BackgroundTask _backgroundReader;
        private PipeReaderWriter _pipeReaderWriter;

        /// <summary>
        /// Gets the connection's pipe name.
        /// </summary>
        public string PipeName { get; }

        /// <summary>
        /// Gets the connection's server name. Only for client connections.
        /// </summary>
        public string ServerName { get; }

        /// <summary>
        /// Gets a value indicating whether the pipe is connected or not.
        /// </summary>
        public bool IsConnected => _pipeStream?.IsConnected ?? false;                   // ?? _pipeReaderWriter is re-instate its' IsConnected


        /// <summary>
        /// Invoked when the named pipe connection terminates.
        /// </summary>
        public event EventHandler<ConnectionEventArgs<TType>> OnDisconnected;

        /// <summary>
        /// Invoked whenever a message is received from the other end of the pipe.
        /// </summary>
        public event EventHandler<ConnectionMessageEventArgs<TType>> OnMessageReceived;

        /// <summary>
        /// Invoked when an exception is thrown during any read/write operation over the named pipe.
        /// </summary>
        public event EventHandler<ConnectionExceptionEventArgs<TType>> OnException;


        internal PipeConnection(PipeStream stream, string pipeName, IMessageSerializer<TType> serializer)
        {
            _pipeStream = stream.WhenNotNull(nameof(stream));
            PipeName = pipeName.WhenNotNullOrEmpty(nameof(pipeName));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }

        internal PipeConnection(PipeStream stream, string pipeName, IMessageSerializer<TType> serializer, string serverName)
            : this(stream, pipeName, serializer)
        {
            ServerName = serverName.WhenNotNullOrEmpty(nameof(serverName));
        }



        /// <summary>
        /// Begins reading from and writing to the named pipe on a background thread.
        /// </summary>
        public void Connect()
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

                        if (bytes == null || !IsConnected)
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

        public async Task DisconnectAsync()
        {
            // _pipeStream is only disposed of in DisposeAsync().
            if (_cancellationTokenSource is not null)
            {
                _cancellationTokenSource.Cancel();

                await _backgroundReader.DisposeAsync().ConfigureAwait(false);
                _backgroundReader = null;

                await _pipeReaderWriter.DisposeAsync().ConfigureAwait(false);
                _pipeReaderWriter = null;

                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        /// <summary>
        /// Writes the specified <paramref name="value"/> and waits other end reading
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        public async Task WriteAsync(TType value, CancellationToken cancellationToken = default)
        {
            if (!IsConnected || !_pipeReaderWriter.CanWrite)
            {
                // TODO : Custom exception - also required in PipeReaderWriter
                throw new InvalidOperationException("Connection is not connected or writable.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            var bytes = _serializer.Serialize(value);

            await _pipeReaderWriter.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Gets the user name of the client on the other end of the pipe.</summary>
        /// <returns>The user name of the client on the other end of the pipe.</returns>
        /// <exception cref="InvalidOperationException"><see cref="_pipeStream"/> is not <see cref="NamedPipeServerStream"/>.</exception>
        /// <exception cref="InvalidOperationException">No pipe connections have been made yet.</exception>
        /// <exception cref="InvalidOperationException">The connected pipe has already disconnected.</exception>
        /// <exception cref="InvalidOperationException">The pipe handle has not been set.</exception>
        /// <exception cref="ObjectDisposedException">The pipe is closed.</exception>
        /// <exception cref="IOException">The pipe connection has been broken.</exception>
        /// <exception cref="IOException">The user name of the client is longer than 19 characters.</exception>
        public string GetImpersonationUserName()
        {
            if (_pipeStream is not NamedPipeServerStream serverStream)
            {
                throw new InvalidOperationException($"{nameof(_pipeStream)} is not {nameof(NamedPipeServerStream)}.");
            }

            return serverStream.GetImpersonationUserName();
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync().ConfigureAwait(false);

            if (_pipeStream is not null)
            {
                await _pipeStream.DisposeAsync().ConfigureAwait(false);
                _pipeStream = null;
            }
            
        }

        private void DoOnDisconnected()
        {
            var onDisconnected = OnDisconnected;

            if ( onDisconnected is not null)
            {
                var args = new ConnectionEventArgs<TType>(this);

                onDisconnected.Invoke(this, args);
            }
        }

        private void DoOnMessageReceived(TType message)
        {
            var onMessageReceived = OnMessageReceived;

            if (onMessageReceived  is not null)
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