using AllOverIt.Fixture;
using AllOverIt.Plugin.Exceptions;
using AllOverIt.Tests.Helpers;
using Xunit;

namespace AllOverIt.Tests.Plugin.Exceptions
{
    public class CannotLoadPluginTypeExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<CannotLoadPluginTypeException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<CannotLoadPluginTypeException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<CannotLoadPluginTypeException>();
        }
    }
}