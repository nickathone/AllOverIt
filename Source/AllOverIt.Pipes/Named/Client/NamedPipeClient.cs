using AllOverIt.Assertion;
using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Named.Server;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.Pipes.Named.Serialization;

namespace AllOverIt.Pipes.Named.Client
{
    public sealed class NamedPipeClient<TMessage> : INamedPipeClient<TMessage>
    {
        private const string LocalServer = ".";

        private readonly INamedPipeSerializer<TMessage> _serializer;
        private INamedPipeClientConnection<TMessage> _connection;


        /// <inheritdoc/>
        public bool IsConnected => _connection?.IsConnected ?? false;


        /// <inheritdoc/>
        public string PipeName { get; }

        /// <inheritdoc/>
        public string ServerName { get; }



        /// <summary>
        /// Invoked whenever a message is received from the server.
        /// </summary>
        public event EventHandler<NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnMessageReceived;

        /// <summary>
        /// Invoked after each the client connect to the server (include reconnects).
        /// </summary>
        public event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnConnected;

        /// <summary>
        /// Invoked when the client disconnects from the server (e.g., the pipe is closed or broken).
        /// </summary>
        public event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnDisconnected;

        /// <summary>
        /// Invoked whenever an exception is thrown during a read or write operation on the named pipe.
        /// </summary>
        public event EventHandler<NamedPipeExceptionEventArgs> OnException;


        /// <summary>
        /// Constructs a new <see cref="NamedPipeClient{T}"/> to connect to the <see cref="NamedPipeServer{T}"/> specified by <paramref name="pipeName"/>.
        /// </summary>
        /// <param name="pipeName">Name of the server's pipe</param>
        /// <param name="serializer"> ... </param>

        // TODO: Create a factory so IOC can be used

        public NamedPipeClient(string pipeName, INamedPipeSerializer<TMessage> serializer)
            : this(pipeName, LocalServer, serializer)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="serverName">The name of the server to communicate with.</param>
        /// <param name="serializer"></param>
        public NamedPipeClient(string pipeName, string serverName, INamedPipeSerializer<TMessage> serializer)
        {
            PipeName = pipeName.WhenNotNullOrEmpty(nameof(pipeName));
            ServerName = serverName.WhenNotNullOrEmpty(nameof(serverName));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }



        /// <summary>
        /// Connects to the named pipe server asynchronously.
        /// </summary>
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Add auto-reconnect if the server is lost

            //while (IsConnecting)
            //{
            //    await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);
            //}

            if (IsConnected)
            {
                return;
            }

            try
            {
                //IsConnecting = true;

                var connectionPipeName = await GetConnectionPipeName(cancellationToken).ConfigureAwait(false);

                // Connect to the actual data pipe
                var connectionStream = await NamedPipeClientFactory
                    .CreateAndConnectAsync(connectionPipeName, ServerName, cancellationToken)
                    .ConfigureAwait(false);


                // TODO: Handle cleanup if exception occurs after this
                _connection = new NamedPipeClientConnection<TMessage>(connectionStream, connectionPipeName, _serializer, ServerName);

                _connection.OnMessageReceived += DoOnConnectionMessageReceived;
                _connection.OnDisconnected += DoOnConnectionDisconnected;
                _connection.OnException += DoOnConnectionException;

                _connection.Connect();

                DoOnConnected(_connection);
            }
            //catch (Exception)
            //{
            //    ReconnectionTimer.Stop();

            //    throw;
            //}
            finally
            {
                //IsConnecting = false;
            }
        }



        /// <summary>
        /// Disconnects from server
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public async Task DisconnectAsync(CancellationToken _ = default)
        {
            await DoOnDisconnectedAsync().ConfigureAwait(false);
        }




        /// <summary>
        /// Sends a message to the server over a named pipe. <br/>
        /// If client is not connected, <see cref="InvalidOperationException"/> is occurred
        /// </summary>
        /// <param name="value">Message to send to the server.</param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task WriteAsync(TMessage value, CancellationToken cancellationToken = default)
        {
            //if (!IsConnected && AutoReconnect)
            //{
            //    await ConnectAsync(cancellationToken).ConfigureAwait(false);
            //}

            if (_connection == null)
            {
                throw new InvalidOperationException("Client is not connected");
            }

            await _connection.WriteAsync(value, cancellationToken).ConfigureAwait(false);
        }



        public async ValueTask DisposeAsync()
        {
            //ReconnectionTimer.Dispose();

            await DoOnDisconnectedAsync().ConfigureAwait(false);
        }



        /// <summary>
        /// Get the name of the data pipe that should be used from now on by this pipe client.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetConnectionPipeName(CancellationToken cancellationToken)
        {
            var reader = await NamedPipeClientFactory
                .ConnectAsync(PipeName, ServerName, cancellationToken)
                .ConfigureAwait(false);

            await using (reader)
            {
                var bytes = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);

                // TODO: Custom exception
                Throw<InvalidOperationException>.When(!bytes.Any(), "Connection handshake failed.");

                return Encoding.UTF8.GetString(bytes);
            }
        }

        private void DoOnConnectionDisconnected(object sender, NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>> args)
        {
            try
            {
                Reactive.TaskHelper.ExecuteAsyncAndWait(DoOnDisconnectedAsync);
            }
            catch (Exception exception)
            {
                DoOnException(exception);
            }

            OnDisconnected?.Invoke(this, args);
        }

        private async Task DoOnDisconnectedAsync()
        {
            if (_connection == null)
            {
                return;
            }

            _connection.OnMessageReceived -= DoOnConnectionMessageReceived;
            _connection.OnDisconnected -= DoOnConnectionDisconnected;
            _connection.OnException -= DoOnConnectionException;

            await _connection.DisposeAsync().ConfigureAwait(false);

            _connection = null;
        }

        private void DoOnConnected(INamedPipeClientConnection<TMessage> connection)
        {
            var onConnected = OnConnected;

            if (onConnected is not null)
            {
                var args = new NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>(connection);

                onConnected.Invoke(this, args);
            }
        }

        private void DoOnConnectionMessageReceived(object sender, NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeClientConnection<TMessage>> args)
        {
            OnMessageReceived?.Invoke(this, args);
        }

        private void DoOnConnectionException(object sender, NamedPipeConnectionExceptionEventArgs<TMessage, INamedPipeClientConnection<TMessage>> args)
        {
            DoOnException(args.Exception);
        }

        private void DoOnException(Exception exception)
        {
            var onException = OnException;

            if (onException is not null)
            {
                var args = new NamedPipeExceptionEventArgs(exception);

                onException.Invoke(this, args);
            }
        }
    }
}