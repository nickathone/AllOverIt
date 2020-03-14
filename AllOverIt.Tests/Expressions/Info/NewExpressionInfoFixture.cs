using AllOverIt.Expressions.Info;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Expressions.Info
{
  public class NewExpressionInfoFixture : AllOverItFixtureBase
  {
    public class Constructor : NewExpressionInfoFixture
    {
      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(
            () => new NewExpressionInfo(null, A.CollectionOfFake<IExpressionInfo>(5)))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Throw_When_Arguments_Null()
      {
        var expression = A.Fake<Expression>();

        Invoking(
            () => new NewExpressionInfo(expression, null))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("arguments"));
      }

      [Fact]
      public void Should_Initialize()
      {
        var expression = A.Fake<Expression>();
        var arguments = A.CollectionOfFake<IExpressionInfo>(5);

        var actual = new NewExpressionInfo(expression, arguments);

        actual.Should().BeEquivalentTo(new
        {
          Expression = expression,
          Arguments = arguments,
          InfoType = ExpressionInfoType.New
        });
      }
    }
  }
}