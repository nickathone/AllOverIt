using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Tests.Helpers;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Exceptions
{
    public class FormulaExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            Fixture.AssertDefaultConstructor<FormulaException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            Fixture.AssertConstructorWithMessage<FormulaException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            Fixture.AssertConstructorWithMessageAndInnerException<FormulaException>();
        }
    }
}