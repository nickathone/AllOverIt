using AllOverIt.Fixture;
using AllOverIt.Reflection.Exceptions;
using AllOverIt.Tests.Helpers;
using Xunit;

namespace AllOverIt.Tests.Reflection.Exceptions
{
    public class ReflectionExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<ReflectionException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<ReflectionException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<ReflectionException>();
        }
    }
}