using AllOverIt.Expressions.Info;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Expressions.Info
{
  public class ExpressionInfoBaseFixture : AllOverItFixtureBase
  {
    private class DummyExpressionInfo : ExpressionInfoBase
    {
      public DummyExpressionInfo(Expression expression, ExpressionInfoType type)
        : base(expression, type)
      {
      }
    }

    public class Constructor : ExpressionInfoBaseFixture
    {
      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(
            () => new DummyExpressionInfo(null, Create<ExpressionInfoType>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Initialize()
      {
        var expression = Expression.Constant(Create<int>());
        var infoType = Create<ExpressionInfoType>();

        var actual = new DummyExpressionInfo(expression, infoType);

        actual.Should().BeEquivalentTo(new
        {
          Expression = expression,
          InfoType = infoType
        });
      }
    }
  }
}