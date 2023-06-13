using AllOverIt.Pipes.Connection;
using AllOverIt.Pipes.Events;
using AllOverIt.Pipes.Extensions;
using AllOverIt.Pipes.Serialization;
using AllOverIt.Pipes.Serialization.Binary;
using AllOverIt.Pipes.Server;
using AllOverIt.Reactive;
using NamedPipeTypes;
using System;
using System.IO;
using System.IO.Pipes;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
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
                using (var source = new CancellationTokenSource())
                {
                    Console.WriteLine($"Running in SERVER mode. PipeName: {pipeName}");
                    Console.WriteLine("Enter 'quit' to exit");

                    await using (var server = new PipeServer<PipeMessage>(pipeName, serializer))
                    {
                        server.OnClientConnected += (_, args) => OnClientConnected(server, args, source.Token);

                        server.OnClientDisconnected += (_, args) =>
                        {
                            Console.WriteLine($"Client {args.Connection.PipeName} disconnected");
                        };

                        server.OnMessageReceived += (_, args) =>
                        {
                            var connection = args.Connection;

                            Console.WriteLine($"Client {connection.PipeName} (as '{connection.GetImpersonationUserName()}') sent: {args.Message}");
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

                        server.Start(pipeSecurity =>
                        {
#pragma warning disable CA1416 // Validate platform compatibility
                            pipeSecurity.AddIdentityAccessRule(WellKnownSidType.BuiltinUsersSid, PipeAccessRights.ReadWrite, AccessControlType.Allow);
#pragma warning restore CA1416 // Validate platform compatibility
                        });

                        Console.WriteLine("Server is started!");

                        // Wait until the user quits
                        await WaitForCancellationAsync(source.Token).ConfigureAwait(false);

                        // When the server is disposed it will shut down or we can explicitly disconnect all clients first
                        await server.StopAsync().ConfigureAwait(false);

                        Console.WriteLine("Disposing Server...");
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                OnExceptionOccurred(exception);
            }

            Console.WriteLine("Server Stopped!");
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

        private static void OnClientConnected(IPipeServer<PipeMessage> server, ConnectionEventArgs<PipeMessage, IPipeServerConnection<PipeMessage>> args,
            CancellationToken cancellationToken)
        {
            var connection = args.Connection;

            TaskHelper.ExecuteAsyncAndWait(async () =>
            {
                Console.WriteLine($"Client {connection.PipeName} is now connected.");

                try
                {
                    Console.WriteLine("Sending welcome message");

                    await connection.WriteAsync(new PipeMessage
                    {
                        Text = "Welcome!"
                    }, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    OnExceptionOccurred(exception);
                }

                Console.WriteLine("Message sent");

                SendConnectionRegularMessages(server, connection);
            });
        }

        private static void SendConnectionRegularMessages(IPipeServer<PipeMessage> server, IPipeConnection<PipeMessage> connection)
        {
            Observable
                .Interval(TimeSpan.FromMilliseconds(25))
                .TakeWhile(_ => server.IsActive && connection.IsConnected)
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