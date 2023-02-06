using AllOverIt.Fixture;
using AllOverIt.Formatters.Exceptions;
using AllOverIt.Tests.Helpers;
using Xunit;

namespace AllOverIt.Tests.Formatters.Exceptions
{
    public class SelfReferenceExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<SelfReferenceException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<SelfReferenceException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<SelfReferenceException>();
        }
    }
}