using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Serialization.Binary.Extensions
{
    public class EnrichedBinaryValueReaderExtensionsFixture : FixtureBase
    {
        public class ReadValue : EnrichedBinaryValueReaderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ValueReader_Null()
            {
                Invoking(() =>
                {
                    _ = EnrichedBinaryValueReaderExtensions.ReadValue<string>(null, A.Fake<IEnrichedBinaryReader>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("valueReader");
            }

            [Fact]
            public void Should_Throw_When_Reader_Null()
            {
                Invoking(() =>
                {
                    _ = EnrichedBinaryValueReaderExtensions.ReadValue<string>(A.Fake<IEnrichedBinaryValueReader>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("reader");
            }

            [Fact]
            public void Should_Read_Value()
            {
                var valueReaderFake = new Fake<IEnrichedBinaryValueReader>();
                var readerFake = A.Fake<IEnrichedBinaryReader>();

                var expected = Create<string>();

                valueReaderFake
                   .CallsTo(fake => fake.ReadValue(readerFake))
                   .Returns(expected);

                var actual = EnrichedBinaryValueReaderExtensions.ReadValue<string>(valueReaderFake.FakedObject, readerFake);

                actual.Should().Be(expected);
            }
        }
    }
}