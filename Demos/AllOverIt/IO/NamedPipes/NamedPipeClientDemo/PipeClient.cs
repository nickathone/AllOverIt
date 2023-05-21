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
                client.OnDisconnected += (o, args) => Console.WriteLine("Disconnected from server");
                client.OnConnected += (o, args) => Console.WriteLine("Connected to server");
                client.OnException += (o, args) => OnExceptionOccurred(args.Exception);

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

                            await client.WriteAsync(new PipeMessage
                            {
                                Text = message
                            }, source.Token).ConfigureAwait(false);
                        }
                        catch (Exception exception)
                        {
                            OnExceptionOccurred(exception);
                        }
                    }
                }, source.Token);

                Console.WriteLine("Client connecting...");

                await client.ConnectAsync(source.Token).ConfigureAwait(false);

                Console.WriteLine("Client connected");

                await Task.Delay(Timeout.InfiniteTimeSpan, source.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }
    }   
}