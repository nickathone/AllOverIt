using AllOverIt.Fixture;
using AllOverIt.Mapping.Exceptions;
using AllOverIt.Tests.Helpers;
using Xunit;

namespace AllOverIt.Mapping.Tests.Exceptions
{
    public class ObjectMapperExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<ObjectMapperException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<ObjectMapperException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<ObjectMapperException>();
        }
    }
}