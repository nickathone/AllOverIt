using AllOverIt.Pipes;
using AllOverIt.Reactive;
using NamedPipeTypes;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipeDemo
{
    public sealed class AsyncCommand
    {
        private readonly Func<Task> _execute;

        public AsyncCommand(Func<Task> execute)
        {
            _execute = execute;
        }

        public void Execute()
        {
            var completionSource = new TaskCompletionSource<bool>();

            _execute.Invoke().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    completionSource.TrySetException(task.Exception);
                }
                else if (task.IsCanceled)
                {
                    completionSource.TrySetCanceled();
                }
                else
                {
                    completionSource.TrySetResult(true);
                }
            });

            completionSource.Task.GetAwaiter().GetResult();
        }
    }


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
                Console.WriteLine("Enter 'q' to exit");

                await using var server = new PipeServer<PipeMessage>(pipeName, serializer);

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
                    while (true)
                    {
                        try
                        {
                            var message = await Console.In.ReadLineAsync().ConfigureAwait(false);

                            if (message == "q")
                            {
                                source.Cancel();
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
                await Task.Delay(Timeout.InfiniteTimeSpan, source.Token).ConfigureAwait(false);

                // Disposing the server will shut it down
                await server.DisposeAsync().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
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
            });
        }
    }
}