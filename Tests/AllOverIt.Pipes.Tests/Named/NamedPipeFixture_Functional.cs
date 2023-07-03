using AllOverIt.Async;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Pipes.Exceptions;
using AllOverIt.Pipes.Named.Client;
using AllOverIt.Pipes.Named.Serialization;
using AllOverIt.Pipes.Named.Server;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Pipes.Tests.Named
{
    public class NamedPipeFixture_Functional : FixtureBase
    {
        private class DummyMessage
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public Guid Guid { get; set; }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Should_Return_Server_Connected_Status(bool connected)
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                if (connected)
                {
                    server.Start();
                }

                server.IsStarted.Should().Be(connected);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Should_Return_Client_Connected_Status(bool connected)
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    if (connected)
                    {
                        await client.ConnectAsync();
                    }

                    client.IsConnected.Should().Be(connected);
                }
            }
        }

        [Fact]
        public async Task Should_Throw_When_Already_Connected()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    await client.ConnectAsync();

                    await Invoking(async () =>
                    {
                        await client.ConnectAsync();
                    })
                        .Should()
                        .ThrowAsync<PipeException>()
                        .WithMessage("The named pipe client is already connected.");

                    client.IsConnected.Should().BeTrue();
                }
            }
        }

        [Fact]
        public async Task Should_Disconnect()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    await client.ConnectAsync();

                    client.IsConnected.Should().BeTrue();

                    await client.DisconnectAsync();

                    client.IsConnected.Should().BeFalse();
                }
            }
        }

        [Fact]
        public async Task Should_Return_Server_PipeName()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.PipeName.Should().Be(pipeName);
            }
        }

        [Fact]
        public async Task Should_Return_Client_PipeName()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    client.PipeName.Should().Be(pipeName);
                }
            }
        }

        [Fact]
        public async Task Should_Send_Message_From_Server_To_Client()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();
            DummyMessage actual = null;

            var tcs = new TaskCompletionSource<DummyMessage>();

            void Client_OnMessageReceived(object sender, Pipes.Named.Events.NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
            {
                tcs.SetResult(eventArgs.Message);
            }

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    client.OnMessageReceived += Client_OnMessageReceived;

                    await client.ConnectAsync();

                    var serverTask = Task.Run(async () =>
                    {
                        await server.WriteAsync(expected, CancellationToken.None).ConfigureAwait(false);
                    });

                    actual = await tcs.Task;
                }
            }

            expected.Should().BeEquivalentTo(actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Should_Send_Message_From_Server_To_Filtered_Client(int connectionIndex)
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();
            var actual = new List<string>();

            var tcs = new TaskCompletionSource<string>();

            void Client_OnMessageReceived(object sender, Pipes.Named.Events.NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
            {
                tcs.SetResult(eventArgs.Message.Value);
            }

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.Start();

                var composites = new CompositeAsyncDisposable();

                async Task<NamedPipeClient< DummyMessage>> CreateClientAsync()
                {
                    var client = new NamedPipeClient<DummyMessage>(pipeName, serializer);

                    composites.Add(client);

                    client.OnMessageReceived += Client_OnMessageReceived;
                    await client.ConnectAsync();

                    return client;
                }

                var client1 = await CreateClientAsync();
                var client2 = await CreateClientAsync();

                await using (composites)
                {
                    var serverTask = Task.Run(async () =>
                    {
                        if (connectionIndex == 2)
                        {
                            var filteredIds = (server as NamedPipeServer<DummyMessage>)
                                .Connections
                                .Select(item => item.ConnectionId)
                                .ToArray();

                            await server.WriteAsync(
                                expected,
                                connection => connection.ConnectionId == filteredIds[0] ||
                                              connection.ConnectionId == filteredIds[1],
                                CancellationToken.None).ConfigureAwait(false);
                        }
                        else
                        {
                            var filteredId = (server as NamedPipeServer<DummyMessage>)
                                .Connections
                                .ElementAt(connectionIndex)
                                .ConnectionId;

                            expected.Value = filteredId;

                            await server.WriteAsync(expected, connection => connection.ConnectionId == filteredId, CancellationToken.None).ConfigureAwait(false);
                        }
                    });

                    var result = await tcs.Task;

                    actual.Add(result);

                    if (connectionIndex != 2)
                    {
                        result.Should().Be(expected.Value);
                    }
                }
            }

            actual.Should().HaveCount(1);
        }

        [Fact]
        public async Task Should_Send_Message_From_Client_To_Server()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = Create<DummyMessage>();
            DummyMessage actual = null;

            var tcs = new TaskCompletionSource<DummyMessage>();

            void Server_OnMessageReceived(object sender, Pipes.Named.Events.NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
            {
                tcs.SetResult(eventArgs.Message);
            }

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.OnMessageReceived += Server_OnMessageReceived;

                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    await client.ConnectAsync();

                    await client.WriteAsync(expected, CancellationToken.None).ConfigureAwait(false);

                    actual = await tcs.Task;
                }
            }

            expected.Should().BeEquivalentTo(actual);
        }

        [Fact]
        public async Task Should_Raise_Client_OnConnected()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var actual = false;

            var tcs = new TaskCompletionSource<bool>();

            void Client_OnConnected(object sender, Pipes.Named.Events.NamedPipeConnectionEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
            {
                tcs.SetResult(true);
            }

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    client.OnConnected += Client_OnConnected;

                    await client.ConnectAsync();

                    actual = await tcs.Task;
                }
            }

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Raise_Client_OnDisconnected()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var actual = false;

            var tcs = new TaskCompletionSource<bool>();

            void Client_OnDisconnected(object sender, Pipes.Named.Events.NamedPipeConnectionEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
            {
                tcs.SetResult(true);
            }

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    client.OnDisconnected += Client_OnDisconnected;

                    await client.ConnectAsync();

                    await server.StopAsync();

                    actual = await tcs.Task;
                }
            }

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Raise_Client_OnMessageReceived()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var actual = false;

            var tcs = new TaskCompletionSource<bool>();

            void Client_OnMessageReceived(object sender, Pipes.Named.Events.NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
            {
                tcs.SetResult(true);
            }

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    client.OnMessageReceived += Client_OnMessageReceived;

                    await client.ConnectAsync();

                    var serverTask = Task.Run(async () =>
                    {
                        await server.WriteAsync(Create<DummyMessage>(), CancellationToken.None);
                    });

                    actual = await tcs.Task;
                }
            }

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Raise_Client_OnException()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = new Exception();
            Exception actual = null;

            var tcs = new TaskCompletionSource<Exception>();

            void Client_OnMessageReceived(object sender, Pipes.Named.Events.NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
            {
                throw expected;
            }

            void Client_OnException(object sender, Pipes.Named.Events.NamedPipeExceptionEventArgs eventArgs)
            {
                tcs.SetResult(eventArgs.Exception);
            }

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    client.OnMessageReceived += Client_OnMessageReceived;
                    client.OnException += Client_OnException;

                    await client.ConnectAsync();

                    var serverTask = Task.Run(async () =>
                    {
                        await server.WriteAsync(Create<DummyMessage>(), CancellationToken.None);
                    });

                    actual = await tcs.Task;
                }
            }

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public async Task Should_Raise_Client_OnException_When_OnDisconnected_Throws()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = new Exception();
            Exception actual = null;

            var tcs = new TaskCompletionSource<Exception>();

            void Client_OnDisconnected(object sender, Pipes.Named.Events.NamedPipeConnectionEventArgs<DummyMessage, INamedPipeClientConnection<DummyMessage>> eventArgs)
            {
                throw expected;
            }

            void Client_OnException(object sender, Pipes.Named.Events.NamedPipeExceptionEventArgs eventArgs)
            {
                tcs.SetResult(eventArgs.Exception);
            }

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    client.OnDisconnected += Client_OnDisconnected;
                    client.OnException += Client_OnException;

                    await client.ConnectAsync();

                    await server.StopAsync();

                    actual = await tcs.Task;

                    client.IsConnected.Should().BeFalse();
                }
            }

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public async Task Should_Raise_Server_OnClientConnected()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var actual = false;

            var tcs = new TaskCompletionSource<bool>();

            void Server_OnClientConnected(object sender, Pipes.Named.Events.NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
            {
                tcs.SetResult(true);
            }

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.OnClientConnected += Server_OnClientConnected;
                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    await client.ConnectAsync();

                    actual = await tcs.Task;
                }
            }

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Raise_Server_OnClientDisconnected()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var actual = false;

            var tcs = new TaskCompletionSource<bool>();

            void Server_OnClientDisconnected(object sender, Pipes.Named.Events.NamedPipeConnectionEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
            {
                tcs.SetResult(true);
            }

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.OnClientDisconnected += Server_OnClientDisconnected;
                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    await client.ConnectAsync();

                    await client.DisconnectAsync();

                    actual = await tcs.Task;
                }
            }

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Raise_Server_OnMessageReceived()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var actual = false;

            var tcs = new TaskCompletionSource<bool>();

            void Server_OnMessageReceived(object sender, Pipes.Named.Events.NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
            {
                tcs.SetResult(true);
            }

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.OnMessageReceived += Server_OnMessageReceived;
                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    await client.ConnectAsync();

                    await client.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

                    actual = await tcs.Task;
                }
            }

            actual.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Raise_Server_OnException_And_Disconnect()
        {
            var pipeName = Create<string>();
            var serializer = new NamedPipeSerializer<DummyMessage>();
            var expected = new Exception();
            Exception actual = null;

            var tcs = new TaskCompletionSource<Exception>();

            void Server_OnMessageReceived(object sender, Pipes.Named.Events.NamedPipeConnectionMessageEventArgs<DummyMessage, INamedPipeServerConnection<DummyMessage>> eventArgs)
            {
                throw expected;
            }

            void Server_OnException(object sender, Pipes.Named.Events.NamedPipeExceptionEventArgs eventArgs)
            {
                tcs.SetResult(eventArgs.Exception);
            }

            await using (var server = new NamedPipeServer<DummyMessage>(pipeName, serializer))
            {
                server.OnMessageReceived += Server_OnMessageReceived;
                server.OnException += Server_OnException;

                server.Start();

                await using (var client = new NamedPipeClient<DummyMessage>(pipeName, serializer))
                {
                    await client.ConnectAsync();

                    await client.WriteAsync(Create<DummyMessage>(), CancellationToken.None);

                    actual = await tcs.Task;
                }
            }

            actual.Should().BeSameAs(expected);
        }
    }
}