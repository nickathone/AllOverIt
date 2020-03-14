using AllOverIt.Expressions.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Expressions.Exceptions
{
  public class ExpressionParserExceptionFixture : AllOverItFixtureBase
  {
    public class Constructor : ExpressionParserExceptionFixture
    {
      [Fact]
      public void Should_Throw_With_Message()
      {
        var message = Create<string>();

        Invoking(() => throw new ExpressionParserException(message))
          .Should()
          .Throw<ExpressionParserException>()
          .WithMessage(message)
          .And
          .InnerException
          .Should()
          .BeNull();
      }

      [Fact]
      public void Should_Throw_With_Inner_Exception()
      {
        var message = Create<string>();
        var innerMessage = Create<string>();
        var innerException = new ArgumentException(innerMessage);

        Invoking(() => throw new ExpressionParserException(message, innerException))
          .Should()
          .Throw<ExpressionParserException>()
          .WithMessage(message)
          .WithInnerException<ArgumentException>()
          .WithMessage(innerMessage);
      }
    }
  }
}