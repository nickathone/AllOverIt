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
    public sealed class PipeServer<TType> : IPipeServer<TType>, IPipeEvents<TType>, IPipeServerEvents<TType>
    {
        private readonly IMessageSerializer<TType> _serializer;
        private IList<IPipeConnection<TType>> Connections { get; } = new List<IPipeConnection<TType>>();
        private IAwaitableLock _connectionsLock = new AwaitableLock();
        private BackgroundTask _backgroundTask;


        public string PipeName { get; }

        /// <summary>An event raised when a client connects to the server. This event may be raised on a background thread.</summary>
        public event EventHandler<ConnectionEventArgs<TType>> OnClientConnected;

        /// <summary>An event raised when a client disconnects from the server. This event may be raised on a background thread.</summary>
        public event EventHandler<ConnectionEventArgs<TType>> OnClientDisconnected;

        /// <summary>
        /// Invoked whenever a client sends a message to the server.
        /// </summary>
        public event EventHandler<ConnectionMessageEventArgs<TType>> OnMessageReceived;

        /// <summary>
        /// Invoked whenever an exception is thrown during a read or write operation.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> OnException;


        // TODO: Create a factory so IOC can be used
        public PipeServer(string pipeName, IMessageSerializer<TType> serializer)
        {
            PipeName = pipeName.WhenNotNullOrEmpty(nameof(pipeName));
            _serializer = serializer.WhenNotNull(nameof(serializer));
        }


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

        public void Start(Action<PipeSecurity> securityConfiguration)
        {
            _ = securityConfiguration.WhenNotNull(nameof(securityConfiguration));

            var security = new PipeSecurity();
            
            securityConfiguration.Invoke(security);

            Start(security);
        }

        public void Start(PipeSecurity pipeSecurity = null)
        {

            // TODO: Throw if already started


            _backgroundTask = new BackgroundTask(async token =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var connectionPipeName = $"{Guid.NewGuid()}";

                        // Send the client the name of the data pipe to use
                        var serverStream = PipeServerFactory.Create(PipeName, pipeSecurity);       // TODO: Allow the factory to provide default security, this will override if not null

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
                        var connectionStream = PipeServerFactory.Create(connectionPipeName);

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
                        var connection = new PipeConnection<TType>(connectionStream, connectionPipeName, _serializer);

                        connection.OnMessageReceived += (_, args) => DoOnMessageReceived(args);
                        connection.OnDisconnected += (_, args) => DoOnClientDisconnected(args);
                        connection.OnException += (_, args) => DoOnConnectionException(args.Exception);

                        connection.Connect();

                        await DoOnClientConnectedAsync(connection);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    // Catch the IOException that is raised if the pipe is broken or disconnected.
                    catch (IOException)
                    {
                        // TODO: should be reported - for example, cannot get impersonated user until data has been read from the stream

                        await Task.Yield();
                    }

                    // allow this to go through the exception handler
                    //catch (Exception exception)
                    //{
                    //    OnExceptionOccurred(exception);
                    //    break;
                    //}
                }
            }, edi => true);        // TODO: Need to raise exception event
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
            // Can't use foreach() as the collection is modified.
            while (Connections.Any())
            {
                var connection = Connections[0];

                // DoOnClientDisconnected() will be invoked, removing it from the list of connections
                await connection.DisconnectAsync().ConfigureAwait(false);
            }
        }

        /// <summary>Asynchronously sends a message to all connected clients.</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        public Task WriteAsync(TType message, CancellationToken cancellationToken = default)
        {
            return WriteAsync(message, _ => true, cancellationToken);
        }

        /// <summary>Asynchronously sends a message to the client with a specfied pipe name..</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="pipeName">The name of the pipe to send the message to. This name is case-insensitive.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        public Task WriteAsync(TType message, string pipeName, CancellationToken cancellationToken = default)
        {
            return WriteAsync(message, connection => connection.PipeName.Equals(pipeName, StringComparison.OrdinalIgnoreCase), cancellationToken);
        }

        /// <summary>Asynchronously sends a message to all connected clients that meet a predicate condition.</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="predicate">The predicate condition to be met.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        public async Task WriteAsync(TType message, Predicate<IPipeConnection<TType>> predicate, CancellationToken cancellationToken = default)
        {
            _ = predicate.WhenNotNull(nameof(predicate));

            // Get potential connections synchronously
            IEnumerable<IPipeConnection<TType>> connections = null;

            using (await _connectionsLock.GetLockAsync(cancellationToken))
            {
                connections = Connections
                    .Where(connection => connection.IsConnected && predicate.Invoke(connection))
                    .ToList();
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

            //await connections.ForEachAsync(async (connection, _) =>
            //{
            //    try
            //    {
            //        await connection.WriteAsync(message, cancellationToken).ConfigureAwait(false);
            //    }
            //    catch
            //    {
            //        // TODO: Report / handle
            //    }
            //}, cancellationToken);
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

        private async Task DoOnClientConnectedAsync(IPipeConnection<TType> connection)
        {
            using (await _connectionsLock.GetLockAsync().ConfigureAwait(false))
            {
                Connections.Add(connection);
            }

            var clientConnected = OnClientConnected;

            if (clientConnected is not null)
            {
                var args = new ConnectionEventArgs<TType>(connection);

                clientConnected.Invoke(this, args);
            }
        }

        private void DoOnClientDisconnected(ConnectionEventArgs<TType> args)
        {
            OnClientDisconnected?.Invoke(this, args);

            Reactive.TaskHelper.ExecuteAsyncAndWait(async () =>
            {
                using (await _connectionsLock.GetLockAsync().ConfigureAwait(false))
                {
                    Connections.Remove(args.Connection);
                }
            });
        }

        private void DoOnMessageReceived(ConnectionMessageEventArgs<TType> args)
        {
            OnMessageReceived?.Invoke(this, args);
        }

        private void DoOnConnectionException(Exception exception)
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