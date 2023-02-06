using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Tests.Helpers;
using Xunit;

namespace AllOverIt.DependencyInjection.Tests.Exceptions
{
    public class DependencyRegistrationExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<DependencyRegistrationException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<DependencyRegistrationException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<DependencyRegistrationException>();
        }
    }
}