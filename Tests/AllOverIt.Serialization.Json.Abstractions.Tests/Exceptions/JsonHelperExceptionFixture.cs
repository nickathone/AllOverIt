using AllOverIt.Fixture;
using AllOverIt.Serialization.Json.Abstractions.Exceptions;
using AllOverIt.Tests.Helpers;
using Xunit;

namespace AllOverIt.Serialization.Json.Abstractions.Tests.Exceptions
{
    public class JsonHelperExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<JsonHelperException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<JsonHelperException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<JsonHelperException>();
        }
    }
}