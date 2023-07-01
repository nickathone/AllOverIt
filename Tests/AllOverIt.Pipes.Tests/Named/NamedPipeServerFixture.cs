using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Named.Serialization;
using AllOverIt.Pipes.Named.Server;
using FakeItEasy;
using FluentAssertions;
using System;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Pipes.Tests.Named
{
    public class NamedPipeServerFixture : FixtureBase
    {
        public sealed class DummyMessage
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public Guid Guid { get; set; }
        }

        public class Constructor : NamedPipeServerFixture
        {
            [Fact]
            public void Should_Throw_When_PipeName_Null()
            {

                Invoking(() =>
                {
                    _ = new NamedPipeServer<DummyMessage>(null, A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("pipeName");
            }

            [Fact]
            public void Should_Throw_When_PipeName_Empty()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeServer<DummyMessage>(string.Empty, A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("pipeName");
            }

            [Fact]
            public void Should_Throw_When_PipeName_Whitespace()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeServer<DummyMessage>("  ", A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("pipeName");
            }

            [Fact]
            public void Should_Throw_When_Serializer_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeServer<DummyMessage>(Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serializer");
            }
        }

        public class Start_Action : NamedPipeServerFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    var server = new NamedPipeServer<DummyMessage>(Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());

                    server.Start((Action<PipeSecurity>) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("pipeSecurity");
            }

            [Fact]
            public void Should_Configure_Security()
            {
                var server = new NamedPipeServer<DummyMessage>(Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());

                PipeSecurity security = null;

                server.Start(pipeSecurity =>
                {
                    security = pipeSecurity;
                });

                security.Should().BeOfType<PipeSecurity>();
            }

            [Fact]
            public void Should_Start_Server()
            {
                var serializer = new NamedPipeSerializer<DummyMessage>();

                var server = new NamedPipeServer<DummyMessage>(Create<string>(), serializer);

                server.Start();

                server.IsStarted.Should().BeTrue();
            }
        }

        public class Start_PipeSecurity : NamedPipeServerFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Security_Null()
            {
                var server = new NamedPipeServer<DummyMessage>(Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());

                Invoking(() =>
                {
                    server.Start((PipeSecurity)null);
                })
                .Should()
                .NotThrow();
            }
        }

        public class Stop : NamedPipeServerFixture
        {
            [Fact]
            public async Task Should_Stop_Server()
            {
                var serializer = new NamedPipeSerializer<DummyMessage>();

                var server = new NamedPipeServer<DummyMessage>(Create<string>(), serializer);

                server.Start();

                server.IsStarted.Should().BeTrue();

                await server.StopAsync();

                server.IsStarted.Should().BeFalse();
            }
        }
    }
}