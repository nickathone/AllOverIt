using AllOverIt.Fixture;
using AllOverIt.Serialization.Abstractions.Exceptions;
using AllOverIt.Tests.Helpers;
using Xunit;

namespace AllOverIt.Serialization.Abstractions.Tests.Exceptions
{
    public class SerializerConfigurationExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<SerializerConfigurationException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<SerializerConfigurationException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<SerializerConfigurationException>();
        }
    }
}