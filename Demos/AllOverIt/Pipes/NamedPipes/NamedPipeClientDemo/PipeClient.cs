using AllOverIt.Pipes.Client;
using AllOverIt.Pipes.Connection;
using AllOverIt.Pipes.Events;
using AllOverIt.Pipes.Serialization;
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

        public static async Task RunAsync(string pipeName, bool useCustomReaderWriter = true)
        {
            // If the custom reader/writers are not register then a DynamicBinaryValueReader / DynamicBinaryValueWriter
            // will be created by EnrichedBinaryReader / EnrichedBinaryWriter. The customer reader / writer will be
            // more efficient in time and space as they can be tailored to the exact shape of the model and avoid
            // writing unnecessary type information.
            IMessageSerializer<PipeMessage> serializer = useCustomReaderWriter
                ? new PipeMessageSerializer()
                : new BinaryMessageSerializer<PipeMessage>();

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
                .Interval(TimeSpan.FromMilliseconds(25))
                .TakeWhile(_ => connection.IsConnected)
                .SelectMany(async async =>
                {
                    try
                    {
                        await connection.WriteAsync(new PipeMessage { Text = $"{DateTime.Now:o}" }).ConfigureAwait(false);
                    }
                    catch (IOException)
                    {
                        // Most likely a broken pipe
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