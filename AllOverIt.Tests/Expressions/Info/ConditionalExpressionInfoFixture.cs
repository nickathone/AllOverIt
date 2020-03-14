using AllOverIt.Expressions.Info;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Expressions.Info
{
  public class ConditionalExpressionInfoFixture : AllOverItFixtureBase
  {
    public class Constructor : ConditionalExpressionInfoFixture
    {
      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(
            () => new ConditionalExpressionInfo(null, A.Fake<IExpressionInfo>(), A.Fake<IExpressionInfo>(), A.Fake<IExpressionInfo>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Throw_When_Test_Null()
      {
        var expression = Expression.Constant(Create<int>());

        Invoking(
            () => new ConditionalExpressionInfo(expression, null, A.Fake<IExpressionInfo>(), A.Fake<IExpressionInfo>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("test"));
      }

      [Fact]
      public void Should_Throw_When_IfTrue_Null()
      {
        var expression = Expression.Constant(Create<int>());

        Invoking(
            () => new ConditionalExpressionInfo(expression, A.Fake<IExpressionInfo>(), null, A.Fake<IExpressionInfo>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("ifTrue"));
      }

      [Fact]
      public void Should_Throw_When_IfFalse_Null()
      {
        var expression = Expression.Constant(Create<int>());

        Invoking(
            () => new ConditionalExpressionInfo(expression, A.Fake<IExpressionInfo>(), A.Fake<IExpressionInfo>(), null))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("ifFalse"));
      }

      [Fact]
      public void Should_Initialize()
      {
        var expression = A.Fake<Expression>();
        var test = A.Fake<IExpressionInfo>();
        var ifTrue = A.Fake<IExpressionInfo>();
        var ifFalse = A.Fake<IExpressionInfo>();

        var actual = new ConditionalExpressionInfo(expression, test, ifTrue, ifFalse);

        actual.Should().BeEquivalentTo(new
        {
          Expression = expression,
          Test = test,
          IfTrue = ifTrue,
          IfFalse = ifFalse,
          InfoType = ExpressionInfoType.Conditional
        });
      }
    }
  }
}