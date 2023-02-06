using AllOverIt.Fixture;
using AllOverIt.Patterns.ValueObject.Exceptions;
using AllOverIt.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Patterns.ValueObject.Exceptions
{
    public class ValueObjectValidationExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Not_Have_Default_Constructor()
        {
            Fixture.AssertNoDefaultConstructor<ValueObjectValidationException>();
        }

        [Fact]
        public void Should_Not_Have_Constructor_With_Message()
        {
            Fixture.AssertNoConstructorWithMessage<ValueObjectValidationException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Value_And_InnerException()
        {
            var attemptedValue = new object();
            var message = Create<string>();

            var exception = new ValueObjectValidationException(attemptedValue, message);

            exception.AttemptedValue.Should().BeSameAs(attemptedValue);
            exception.Message.Should().Be(message);
        }
    }
}