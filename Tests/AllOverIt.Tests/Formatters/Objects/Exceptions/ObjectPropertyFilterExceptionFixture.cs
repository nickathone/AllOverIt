using AllOverIt.Fixture;
using AllOverIt.Formatters.Objects.Exceptions;
using AllOverIt.Tests.Helpers;
using Xunit;

namespace AllOverIt.Tests.Formatters.Objects.Exceptions
{
    public class ObjectPropertyFilterExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<ObjectPropertyFilterException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<ObjectPropertyFilterException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<ObjectPropertyFilterException>();
        }
    }
}