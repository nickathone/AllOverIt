using AllOverIt.Pipes;
using NamedPipeTypes;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
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
                Console.WriteLine("Enter 'quit' to exit");

                await using var client = new PipeClient<PipeMessage>(pipeName, serializer);

                client.OnMessageReceived += (o, args) => Console.WriteLine("MessageReceived: " + args.Message);

                client.OnDisconnected += (o, args) => Console.WriteLine("Disconnected from server");

                client.OnConnected += Client_OnConnected;

                client.OnException += (o, args) => OnExceptionOccurred(args.Exception);

                _ = Task.Run(async () =>
                {
                    while (!source.Token.IsCancellationRequested)
                    {
                        try
                        {
                            Console.WriteLine("Waiting for user input");

                            var message = await Console.In.ReadLineAsync().ConfigureAwait(false);

                            if (message == "quit")
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

        private static void Client_OnConnected(object sender, ConnectionEventArgs<PipeMessage> args)
        {
            Console.WriteLine("Connected to server");

            SendConnectionRegularMessages(args.Connection);
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
                        // Server has most likely broken the connection
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