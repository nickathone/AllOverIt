using AllOverIt.Assertion;
using AllOverIt.Async;
using AllOverIt.Extensions;
using AllOverIt.Threading;
using AllOverIt.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes
{
    public sealed class PipeServer<TType> : IPipeServer<TType>, IPipeEvents<TType>, IPipeServerEvents<TType>
    {
        private BackgroundTask _backgroundTask;

        public string PipeName { get; }


        private readonly IMessageSerializer<TType> _serializer;

        private IList<IPipeConnection<TType>> Connections { get; } = new List<IPipeConnection<TType>>();
        private IAwaitableLock _connectionsLock = new AwaitableLock();



        /// <summary>
        /// Invoked whenever a client connects to the server.
        /// </summary>
        public event EventHandler<ConnectionEventArgs<TType>> ClientConnected;

        /// <summary>
        /// Invoked whenever a client disconnects from the server.
        /// </summary>
        public event EventHandler<ConnectionEventArgs<TType>> ClientDisconnected;

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


        public void Start()
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
                        var serverStream = //CreatePipeStreamFunc?.Invoke(PipeName) ??              - applies security options
                            PipeServerFactory.Create(PipeName);

                        await using (serverStream)
                        {
                            //check out use
                            //PipeStreamInitializeAction?.Invoke(serverStream);


                            await serverStream.WaitForConnectionAsync(token).ConfigureAwait(false);

                            await using (var writer = new PipeReaderWriter(serverStream))
                            {
                                await writer
                                    .WriteAsync(Encoding.UTF8.GetBytes(connectionPipeName), token)
                                    .ConfigureAwait(false);
                            }
                        }

                        // Wait for the client to connect to the data pipe
                        var connectionStream = //CreatePipeStreamFunc?.Invoke(connectionPipeName) ??   - applies security options
                            PipeServerFactory.Create(connectionPipeName);

                        //check out use
                        //PipeStreamInitializeAction?.Invoke(connectionStream);

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

                        using (await _connectionsLock.GetLockAsync(token))
                        {
                            Connections.Add(connection);
                        }

                        DoOnClientConnected(new ConnectionEventArgs<TType>(connection));
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
                var connection = Connections.First();

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

            await connections.ForEachAsync(async (connection, _) =>
            {
                try
                {
                    await connection.WriteAsync(message, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    // TODO: Report / handle
                }
            }, cancellationToken);
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


        private void DoOnClientConnected(ConnectionEventArgs<TType> args)
        {
            ClientConnected?.Invoke(this, args);
        }

        private void DoOnClientDisconnected(ConnectionEventArgs<TType> args)
        {
            ClientDisconnected?.Invoke(this, args);

            Reactive.TaskHelper.ExecuteAsyncAndWait(async () =>
            {
                using (await _connectionsLock.GetLockAsync())
                {
                    var connection = args.Connection;

                    Connections.Remove(connection);
                }
            });
        }

        private void DoOnMessageReceived(ConnectionMessageEventArgs<TType> args)
        {
            OnMessageReceived?.Invoke(this, args);
        }

        private void DoOnConnectionException(Exception exception)
        {
            OnException?.Invoke(this, new ExceptionEventArgs(exception));
        }
    }
}