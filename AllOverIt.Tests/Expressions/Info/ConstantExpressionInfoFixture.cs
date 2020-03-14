using AllOverIt.Expressions.Info;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Expressions.Info
{
  public class ConstantExpressionInfoFixture : AllOverItFixtureBase
  {
    public class Constructor : ConstantExpressionInfoFixture
    {
      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(
            () => new ConstantExpressionInfo(null, Create<int>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Initialize()
      {
        var value = Create<int>();
        var expression = Expression.Constant(value);

        var actual = new ConstantExpressionInfo(expression, value);

        actual.Should().BeEquivalentTo(new
        {
          Expression = expression,
          InfoType = ExpressionInfoType.Constant,
          Value = value
        });
      }
    }
  }
}