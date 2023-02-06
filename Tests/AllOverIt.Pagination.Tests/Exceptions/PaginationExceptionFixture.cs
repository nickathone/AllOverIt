using AllOverIt.Fixture;
using AllOverIt.Pagination.Exceptions;
using AllOverIt.Tests.Helpers;
using Xunit;

namespace AllOverIt.Pagination.Tests.Exceptions
{
    public class PaginationExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<PaginationException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<PaginationException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<PaginationException>();
        }
    }
}