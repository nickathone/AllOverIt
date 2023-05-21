using AllOverIt.Pipes;
using AllOverIt.Reactive;
using NamedPipeTypes;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipeDemo
{
    internal static class PipeServer
    {
        private static void OnExceptionOccurred(Exception exception)
        {
            Console.Error.WriteLine($"Exception: {exception}");
        }

        public static async Task RunAsync(string pipeName)
        {
            var serializer = new BinaryMessageSerializer<PipeMessage>();
            serializer.Readers.Add(new PipeMessageReader());
            serializer.Writers.Add(new PipeMessageWriter());

            try
            {
                using var source = new CancellationTokenSource();

                Console.WriteLine($"Running in SERVER mode. PipeName: {pipeName}");
                Console.WriteLine("Enter 'quit' to exit");

                await using (var server = new PipeServer<PipeMessage>(pipeName, serializer))
                {
                    server.ClientConnected += (_, args) => OnClientConnected(args, source.Token);

                    server.ClientDisconnected += (_, args) =>
                    {
                        Console.WriteLine($"Client {args.Connection.PipeName} disconnected");
                    };

                    server.OnMessageReceived += (_, args) =>
                    {
                        Console.WriteLine($"Client {args.Connection.PipeName} says: {args.Message}");
                    };

                    server.OnException += (_, args) => OnExceptionOccurred(args.Exception);

                    _ = Task.Run(async () =>
                    {
                        while (!source.Token.IsCancellationRequested)
                        {
                            try
                            {
                                var message = await Console.In.ReadLineAsync().ConfigureAwait(false);

                                if (message == "quit")
                                {
                                    source.Cancel();

                                    Console.WriteLine("Quiting...");

                                    break;
                                }

                                // Console.WriteLine($"Sent to {server.ConnectedClients.Count} clients");

                                await server.WriteAsync(new PipeMessage
                                {
                                    Text = message,
                                }, source.Token).ConfigureAwait(false);
                            }
                            catch (Exception exception)
                            {
                                OnExceptionOccurred(exception);
                            }
                        }
                    }, source.Token);

                    Console.WriteLine("Server starting...");

                    server.Start();

                    Console.WriteLine("Server is started!");

                    // Wait until the user quites with 'q'
                    await WaitForCancellationAsync(source.Token).ConfigureAwait(false);

                    // When the server is disposed it will shut down
                    Console.WriteLine("Stopping Server...!");
                }

                Console.WriteLine("Server Stopped!");
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private static async Task WaitForCancellationAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static void OnClientConnected(ConnectionEventArgs<PipeMessage> args, CancellationToken cancellationToken)
        {
            TaskHelper.ExecuteAsyncAndWait(async () =>
            {
                Console.WriteLine($"Client {args.Connection.PipeName} is now connected!");

                try
                {
                    Console.WriteLine("Sending welcome message");

                    await args.Connection.WriteAsync(new PipeMessage
                    {
                        Text = "Welcome!"
                    }, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    OnExceptionOccurred(exception);
                }

                Console.WriteLine("Message sent");

                SendConnectionRegularMessages(args.Connection);
            });
        }

        private static void SendConnectionRegularMessages(IPipeConnection<PipeMessage> connection)
        {
            Observable
                .Interval(TimeSpan.FromMilliseconds(100))
                .TakeWhile(_ => connection.IsConnected)
                .SelectMany(async async =>
                {
                    try
                    {
                        await connection.WriteAsync(new PipeMessage { Text = $"{DateTime.Now:o}" }).ConfigureAwait(false);
                    }
                    catch (IOException)
                    {
                        // Client has most likely broken the connection
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                    }

                    return Unit.Default;
                })
                .Subscribe();
        }
    }
}