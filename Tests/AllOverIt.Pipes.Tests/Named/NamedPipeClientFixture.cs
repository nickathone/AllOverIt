using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Pipes.Named.Client;
using AllOverIt.Pipes.Named.Serialization;
using AllOverIt.Pipes.Named.Server;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Pipes.Tests.Named
{
    public class NamedPipeClientFixture : FixtureBase
    {
        public sealed class DummyMessage
        {
            public int Id { get; set; }
            public string Value { get; set; }
            public Guid Guid { get; set; }
        }

        public class Constructor_PipeName_Serializer : NamedPipeClientFixture
        {
            [Fact]
            public void Should_Throw_When_PipeName_Null()
            {

                Invoking(() =>
                {
                    _ = new NamedPipeClient<DummyMessage>(null, A.Fake<INamedPipeSerializer<DummyMessage>>());
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
                    _ = new NamedPipeClient<DummyMessage>(string.Empty, A.Fake<INamedPipeSerializer<DummyMessage>>());
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
                    _ = new NamedPipeClient<DummyMessage>("  ", A.Fake<INamedPipeSerializer<DummyMessage>>());
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
                    _ = new NamedPipeClient<DummyMessage>(Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serializer");
            }
        }

        public class Constructor_PipeName_DomainName_Serializer : NamedPipeClientFixture
        {
            [Fact]
            public void Should_Throw_When_PipeName_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClient<DummyMessage>(null, Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());
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
                    _ = new NamedPipeClient<DummyMessage>(string.Empty, Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());
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
                    _ = new NamedPipeClient<DummyMessage>("  ", Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("pipeName");
            }

            [Fact]
            public void Should_Throw_When_DomainName_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClient<DummyMessage>(Create<string>(), null, A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serverName");
            }

            [Fact]
            public void Should_Throw_When_DomainName_Empty()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClient<DummyMessage>(Create<string>(), string.Empty, A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("serverName");
            }

            [Fact]
            public void Should_Throw_When_DomainName_Whitespace()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClient<DummyMessage>(Create<string>(), "  ", A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("serverName");
            }

            [Fact]
            public void Should_Throw_When_Serializer_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClient<DummyMessage>(Create<string>(), Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serializer");
            }
        }

        // Remaining tests in NamedPipeFixture_Functional
    }
}