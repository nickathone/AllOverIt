using AllOverIt.Pipes;
using NamedPipeTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NamedPipeDemo
{
    internal static class PipeClient
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

                Console.WriteLine($"Running in CLIENT mode. PipeName: {pipeName}");
                Console.WriteLine("Enter 'q' to exit");

                await using var client = new PipeClient<PipeMessage>(pipeName, serializer);

                client.OnMessageReceived += (o, args) => Console.WriteLine("MessageReceived: " + args.Message);

                client.OnDisconnected += (o, args) =>
                {
                    Console.WriteLine("Disconnected from server");
                    source.Cancel();
                };

                client.OnConnected += (o, args) => Console.WriteLine("Connected to server");

                client.OnException += (o, args) => OnExceptionOccurred(args.Exception);

                _ = Task.Run(async () =>
                {
                    while (!source.Token.IsCancellationRequested)
                    {
                        try
                        {
                            Console.WriteLine("Waiting for user input");

                            var message = await Console.In.ReadLineAsync().ConfigureAwait(false);

                            if (message == "q")
                            {
                                source.Cancel();
                                break;
                            }

                            Console.WriteLine($"Sending message: {message}");

                            await client
                                .WriteAsync(new PipeMessage
                                {
                                    Text = message
                                }, source.Token)
                                .ConfigureAwait(false);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine($"To be removed: {exception.Message}");

                            OnExceptionOccurred(exception);
                            source.Cancel();
                        }

                        Console.WriteLine("User input processed");
                    }
                }, source.Token);

                Console.WriteLine("Client connecting...");

                await client.ConnectAsync(source.Token).ConfigureAwait(false);

                Console.WriteLine("Client connected");

                // Wait until the user quites with 'q'
                await WaitForCancellationAsync(source.Token).ConfigureAwait(false);

                // When the client is disposed it will shut down
                Console.WriteLine("Disposing Client...");
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
    }   
}