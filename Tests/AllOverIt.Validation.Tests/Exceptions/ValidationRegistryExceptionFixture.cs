using AllOverIt.Fixture;
using AllOverIt.Tests.Helpers;
using AllOverIt.Validation.Exceptions;
using Xunit;

namespace AllOverIt.Serialization.Tests.Exceptions
{
    public class ValidationRegistryExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<ValidationRegistryException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<ValidationRegistryException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<ValidationRegistryException>();
        }
    }
}