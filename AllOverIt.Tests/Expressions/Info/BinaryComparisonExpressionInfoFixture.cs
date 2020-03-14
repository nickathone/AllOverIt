using AllOverIt.Expressions.Info;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Expressions.Info
{
  public class BinaryComparisonExpressionInfoFixture : AllOverItFixtureBase
  {
    public class Constructor : BinaryComparisonExpressionInfoFixture
    {
      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(
            () => new BinaryComparisonExpressionInfo(null, A.Fake<IExpressionInfo>(), A.Fake<IExpressionInfo>(), Create<ExpressionType>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Throw_When_Left_Null()
      {
        var expression = Expression.Constant(Create<int>());

        Invoking(
            () => new BinaryComparisonExpressionInfo(expression, null, A.Fake<IExpressionInfo>(), Create<ExpressionType>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("left"));
      }

      [Fact]
      public void Should_Throw_When_Right_Null()
      {
        var expression = Expression.Constant(Create<int>());

        Invoking(
            () => new BinaryComparisonExpressionInfo(expression, A.Fake<IExpressionInfo>(), null, Create<ExpressionType>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("right"));
      }

      [Fact]
      public void Should_Initialize()
      {
        var expression = Expression.Constant(Create<int>());
        var lhs = A.Fake<IExpressionInfo>();
        var rhs = A.Fake<IExpressionInfo>();
        var operatorType = Create<ExpressionType>();

        var actual = new BinaryComparisonExpressionInfo(expression, lhs, rhs, operatorType);

        actual.Should().BeEquivalentTo(new
        {
          Expression = expression,
          Left = lhs,
          Right = rhs,
          OperatorType = operatorType,
          InfoType = ExpressionInfoType.BinaryComparison
        });
      }
    }
  }
}