using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Tests.Helpers;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Exceptions
{
    public class OperationFactoryExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<OperationFactoryException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<OperationFactoryException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<OperationFactoryException>();
        }
    }
}