using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Serialization.Binary.Extensions
{
    public class EnrichedBinaryValueWriterExtensionsFixture : FixtureBase
    {
        public class WriteValue : EnrichedBinaryValueWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ValueWriter_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryValueWriterExtensions.WriteValue<string>(null, this.CreateStub<IEnrichedBinaryWriter>(), Create<string>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("valueWriter");
            }

            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryValueWriterExtensions.WriteValue<string>(this.CreateStub<IEnrichedBinaryValueWriter>(), null, Create<string>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var valueWriterFake = new Fake<IEnrichedBinaryValueWriter>();
                var writerFake = this.CreateStub<IEnrichedBinaryWriter>();

                var expected = Create<string>();
                string actual = default;

                valueWriterFake
                   .CallsTo(fake => fake.WriteValue(writerFake, expected))
                    .Invokes(call => actual = call.Arguments.Get<string>(1));

                EnrichedBinaryValueWriterExtensions.WriteValue<string>(valueWriterFake.FakedObject, writerFake, expected);

                actual.Should().Be(expected);
            }
        }
    }
}