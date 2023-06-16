using AllOverIt.Assertion;
using AllOverIt.Async;
using AllOverIt.Extensions;
using AllOverIt.Pipes.Connection;
using AllOverIt.Pipes.Events;
using AllOverIt.Pipes.Serialization;
using AllOverIt.Threading;
using AllOverIt.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Server
{
    /// <summary>A named pipe server that can broadcast a strongly type message to all connected clients
    /// as well as receive messages from those clients.</summary>
    /// <typeparam name="TMessage"></typeparam>
    public sealed class PipeServer<TMessage> : IPipeServer<TMessage>
    {
        private readonly IMessageSerializer<TMessage> _serializer;
        private IList<IPipeServerConnection<TMessage>> Connections { get; } = new List<IPipeServerConnection<TMessage>>();
        private IAwaitableLock _connectionsLock = new AwaitableLock();
        private BackgroundTask _backgroundTask;

        /// <inheritdoc />
        public string PipeName { get; }

        /// <inheritdoc />
        public bool IsActive
        {
            get
            {
                Task task = _backgroundTask;

                return task is not null &&
                    !task.IsCompleted &&
                    !task.IsCanceled &&
                    !task.IsFaulted;
            }
        }

        /// <summary>An event raised when a client connects to the server. This event may be raised on a background thread.</summary>
        public event EventHandler<ConnectionEventArgs<TMessage, IPipeServerConnection<TMessage>>> OnClientConnected;

        /// <summary>An event raised when a client disconnects from the server. This event may be raised on a background thread.</summary>
        public event EventHandler<ConnectionEventArgs<TMessage, IPipeServerConnection<TMessage>>> OnClientDisconnected;

        /// <summary>An event raised when a client sends a message to the server. This event may be raised on a background thread.</summary>
        public event EventHandler<ConnectionMessageEventArgs<TMessage, IPipeServerConnection<TMessage>>> OnMessageReceived;

        /// <summary>An event raised when an exception is thrown during a read or write operation.</summary>
        public event EventHandler<ExceptionEventArgs> OnException;


        // TODO: Create a factory so IOC can be used
        public PipeServer(string pipeName, IMessageSerializer<TMessage> serializer)
        {
            PipeName = pipeName.WhenNotNullOrEmpty(nameof(pipeName));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }

        public void Start(Action<PipeSecurity> pipeSecurity)
        {
            _ = pipeSecurity.WhenNotNull(nameof(pipeSecurity));

            var security = new PipeSecurity();
            
            pipeSecurity.Invoke(security);

            Start(security);
        }

        public void Start(PipeSecurity pipeSecurity = null)
        {

            // TODO: Throw if already started


            _backgroundTask = new BackgroundTask(async token =>
            {
                while (!token.IsCancellationRequested)
                {
                    PipeServerConnection<TMessage> connection = null;

                    try
                    {
                        var connectionPipeName = $"{Guid.NewGuid()}";

                        // Send the client the name of the data pipe to use
                        var serverStream = PipeServerFactory.CreateNamedPipeServerStream(PipeName, pipeSecurity);       // TODO: Allow the factory to provide default security, this will override if not null

                        await using (serverStream)
                        {
                            await serverStream.WaitForConnectionAsync(token).ConfigureAwait(false);

                            await using (var writer = new PipeReaderWriter(serverStream, false))
                            {
                                await writer
                                    .WriteAsync(Encoding.UTF8.GetBytes(connectionPipeName), token)
                                    .ConfigureAwait(false);
                            }
                        }

                        // Wait for the client to connect to the data pipe
                        var connectionStream = PipeServerFactory.CreateNamedPipeServerStream(connectionPipeName, pipeSecurity);

                        try
                        {
                            await connectionStream.WaitForConnectionAsync(token).ConfigureAwait(false);
                        }
                        catch
                        {
                            await connectionStream.DisposeAsync().ConfigureAwait(false);

                            throw;
                        }

                        // Add the client's connection to the list of connections
                        connection = new PipeServerConnection<TMessage>(connectionStream, connectionPipeName, _serializer);

                        connection.OnMessageReceived += DoOnConnectionMessageReceived;
                        connection.OnDisconnected += DoOnConnectionDisconnected;
                        connection.OnException += DoOnConnectionException;

                        connection.Connect();

                        await DoOnClientConnectedAsync(connection);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception exception)
                    {
                        if (connection is not null)
                        {
                            await connection.DisposeAsync().ConfigureAwait(false);
                        }

                        DoOnException(exception);
                    }
                }
            });
        }

        public async Task StopAsync()
        {
            // Prevent new connections from being established
            if (_backgroundTask is not null)
            {
                await _backgroundTask.DisposeAsync().ConfigureAwait(false);
                _backgroundTask = null;
            }

            // We don't need to lock since no new connections are possible.
            if (Connections.Any())
            {
                // DoOnConnectionDisconnected() will be called for each connection where
                // its' event handlers are also released.
                await Connections.DisposeAllAsync().ConfigureAwait(false);
            }
        }

        /// <summary>Asynchronously sends a message to all connected clients.</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        public Task WriteAsync(TMessage message, CancellationToken cancellationToken = default)
        {
            return WriteAsync(message, _ => true, cancellationToken);
        }

        /// <summary>Asynchronously sends a message to the client with a specfied pipe name..</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="pipeName">The name of the pipe to send the message to. This name is case-insensitive.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        public Task WriteAsync(TMessage message, string pipeName, CancellationToken cancellationToken = default)
        {
            return WriteAsync(message, connection => connection.PipeName.Equals(pipeName, StringComparison.OrdinalIgnoreCase), cancellationToken);
        }

        /// <summary>Asynchronously sends a message to all connected clients that meet a predicate condition.</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="predicate">The predicate condition to be met.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        public async Task WriteAsync(TMessage message, Predicate<IPipeConnection<TMessage>> predicate, CancellationToken cancellationToken = default)
        {
            _ = predicate.WhenNotNull(nameof(predicate));

            // Get potential connections synchronously
            IEnumerable<IPipeConnection<TMessage>> connections = null;

            using (await _connectionsLock.GetLockAsync(cancellationToken))
            {
                connections = Connections
                    .Where(connection => connection.IsConnected && predicate.Invoke(connection))
                    .AsReadOnlyCollection();
            }

            // TODO: Make the degree of parallelism configurable
            await connections.ForEachAsParallelAsync(async connection =>
            {
                try
                {
                    await connection.WriteAsync(message, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    // TODO: Report / handle
                }

            }, 4, cancellationToken);
        }

        /// <summary>Stops the pipe server and releases resources.</summary>
        public async ValueTask DisposeAsync()
        {
            if (_connectionsLock is not null)
            {
                await StopAsync().ConfigureAwait(false);

                _connectionsLock.Dispose();
                _connectionsLock = null;
            }
        }

        private async Task DoOnClientConnectedAsync(IPipeServerConnection<TMessage> connection)
        {
            using (await _connectionsLock.GetLockAsync().ConfigureAwait(false))
            {
                Connections.Add(connection);
            }

            var clientConnected = OnClientConnected;

            if (clientConnected is not null)
            {
                var args = new ConnectionEventArgs<TMessage, IPipeServerConnection<TMessage>>(connection);

                clientConnected.Invoke(this, args);
            }
        }

        private void DoOnConnectionMessageReceived(object sender, ConnectionMessageEventArgs<TMessage, IPipeServerConnection<TMessage>> args)
        {
            OnMessageReceived?.Invoke(this, args);
        }

        private void DoOnConnectionDisconnected(object sender, ConnectionEventArgs<TMessage, IPipeServerConnection<TMessage>> args)
        {
            OnClientDisconnected?.Invoke(this, args);

            Reactive.TaskHelper.ExecuteAsyncAndWait(async () =>
            {
                var connection = args.Connection;

                connection.OnMessageReceived -= DoOnConnectionMessageReceived;
                connection.OnDisconnected -= DoOnConnectionDisconnected;
                connection.OnException -= DoOnConnectionException;

                using (await _connectionsLock.GetLockAsync().ConfigureAwait(false))
                {
                    Connections.Remove(connection);
                }
            });
        }

        private void DoOnConnectionException(object sender, ConnectionExceptionEventArgs<TMessage, IPipeServerConnection<TMessage>> args)
        {
            DoOnException(args.Exception);
        }

        private void DoOnException(Exception exception)
        {
            var onException = OnException;

            if (onException is not null)
            {
                var args = new ExceptionEventArgs(exception);

                onException.Invoke(this, args);
            }
        }
    }
}