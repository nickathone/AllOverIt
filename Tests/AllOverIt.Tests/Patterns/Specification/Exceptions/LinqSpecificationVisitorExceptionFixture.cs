using AllOverIt.Fixture;
using AllOverIt.Patterns.Specification.Exceptions;
using AllOverIt.Tests.Helpers;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Patterns.Specification.Exceptions
{
    public class LinqSpecificationVisitorExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Not_Have_Default_Constructor()
        {
            Fixture.AssertNoDefaultConstructor<LinqSpecificationVisitorException>();
        }

        [Fact]
        public void Should_Not_Have_Constructor_With_Message()
        {
            Fixture.AssertNoConstructorWithMessage<LinqSpecificationVisitorException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException_And_PartialQueryString()
        {
            var message = Create<string>();
            var innerException = new Exception();
            var partialQueryString = Create<string>();

            var exception = new LinqSpecificationVisitorException(message, innerException, partialQueryString);

            exception.Message.Should().Be(message);
            exception.InnerException.Should().BeSameAs(innerException);
            exception.PartialQueryString.Should().Be(partialQueryString);
        }
    }
}