using AllOverIt.Fixture;
using AllOverIt.Pipes.Exceptions;
using AllOverIt.Pipes.Named.Client;
using AllOverIt.Pipes.Named.Serialization;
using AllOverIt.Pipes.Named.Server;
using FluentAssertions;
using System;
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
    }
}