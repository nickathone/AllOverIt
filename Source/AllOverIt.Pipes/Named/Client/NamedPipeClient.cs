using AllOverIt.Assertion;
using AllOverIt.Pipes.Exceptions;
using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Named.Serialization;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Named.Client
{
    /// <summary>A named pipe client that can serialize messages of type <typeparamref name="TMessage"/> with a named pipe server.</summary>
    /// <typeparam name="TMessage">The message type serialized between a named pipe client and a named pipe server.</typeparam>
    public sealed class NamedPipeClient<TMessage> : INamedPipeClient<TMessage> where TMessage : class, new()
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

        /// <inheritdoc/>
        public event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnConnected;

        /// <inheritdoc/>
        public event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnDisconnected;

        /// <inheritdoc/>
        public event EventHandler<NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnMessageReceived;

        /// <inheritdoc/>
        public event EventHandler<NamedPipeExceptionEventArgs> OnException;

        /// <summary>Constructor.</summary>
        /// <param name="pipeName">The name of the pipe.</param>
        /// <param name="serializer">The serializer to be used by named pipe client instances.</param>
        public NamedPipeClient(string pipeName, INamedPipeSerializer<TMessage> serializer)
            : this(pipeName, LocalServer, serializer)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="pipeName">The name of the pipe.</param>
        /// <param name="serverName">The name of the server to communicate with.</param>
        /// <param name="serializer">The serializer to be used by named pipe client instances.</param>
        public NamedPipeClient(string pipeName, string serverName, INamedPipeSerializer<TMessage> serializer)
        {
            PipeName = pipeName.WhenNotNullOrEmpty(nameof(pipeName));
            ServerName = serverName.WhenNotNullOrEmpty(nameof(serverName));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }

        /// <summary>Asynchronously connects to the named pipe server.</summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task that completes when the connection has been established.</returns>
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            Throw<PipeException>.When(IsConnected, "The named pipe client is already connected.");

            var connectionPipeName = await GetConnectionPipeName(cancellationToken).ConfigureAwait(false);

            var connectionStream = await NamedPipeClientStreamFactory
                .CreateConnectedStreamAsync(connectionPipeName, ServerName, cancellationToken)
                .ConfigureAwait(false);

            _connection = new NamedPipeClientConnection<TMessage>(connectionStream, connectionPipeName, ServerName, _serializer);

            _connection.OnMessageReceived += DoOnConnectionMessageReceived;
            _connection.OnDisconnected += DoOnConnectionDisconnected;
            _connection.OnException += DoOnConnectionException;

            _connection.Connect();

            DoOnConnected(_connection);
        }

        /// <summary>Asynchronously disconnects from the named pipe server.</summary>
        /// <returns>A task that completes when the connection has been disconnected.</returns>
        public async Task DisconnectAsync()
        {
            await DoOnDisconnectedAsync().ConfigureAwait(false);
        }

        /// <summary>Sends a message to the named pipe server.</summary>
        /// <param name="message">The message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task WriteAsync(TMessage message, CancellationToken cancellationToken = default)
        {
            await _connection.WriteAsync(message, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Disconnects from the server if it's connected and disposes of resources.</summary>
        /// <returns>A task that completes when the client has been disposed.</returns>
        public async ValueTask DisposeAsync()
        {
            await DoOnDisconnectedAsync().ConfigureAwait(false);
        }

        private async Task<string> GetConnectionPipeName(CancellationToken cancellationToken)
        {
            var reader = await NamedPipeClientStreamFactory
                .CreateConnectedReaderWriterAsync(PipeName, ServerName, cancellationToken)
                .ConfigureAwait(false);

            await using (reader)
            {
                var bytes = await reader.ReadAsync(cancellationToken).ConfigureAwait(false);

                // TODO: Custom exception
                Throw<InvalidOperationException>.When(!bytes.Any(), "Connection handshake failed.");

                return Encoding.UTF8.GetString(bytes);
            }
        }

        private async void DoOnConnectionDisconnected(object sender, NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>> args)
        {
            try
            {
                await DoOnDisconnectedAsync();

                OnDisconnected?.Invoke(this, args);
            }
            catch (Exception exception)
            {
                DoOnException(exception);
            }
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