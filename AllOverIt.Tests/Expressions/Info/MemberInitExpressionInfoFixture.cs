using AllOverIt.Expressions.Info;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Expressions.Info
{
  public class MemberInitExpressionInfoFixture : AllOverItFixtureBase
  {
    public class Constructor : MemberInitExpressionInfoFixture
    {
      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(
            () => new MemberInitExpressionInfo(null, A.CollectionOfFake<IExpressionInfo>(5), A.CollectionOfFake<IExpressionInfo>(5)))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Throw_When_Bindings_Null()
      {
        var expression = A.Fake<Expression>();

        Invoking(
            () => new MemberInitExpressionInfo(expression, null, A.CollectionOfFake<IExpressionInfo>(5)))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("bindings"));
      }

      [Fact]
      public void Should_Throw_When_Arguments_Null()
      {
        var expression = A.Fake<Expression>();

        Invoking(
            () => new MemberInitExpressionInfo(expression, A.CollectionOfFake<IExpressionInfo>(5), null))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("arguments"));
      }

      [Fact]
      public void Should_Initialize()
      {
        var expression = A.Fake<Expression>();
        var bindings = A.CollectionOfFake< IExpressionInfo>(5);
        var arguments = A.CollectionOfFake<IExpressionInfo>(5);

        var actual = new MemberInitExpressionInfo(expression, bindings, arguments);

        actual.Should().BeEquivalentTo(new
        {
          Expression = expression,
          Bindings = bindings,
          Arguments = arguments,
          InfoType = ExpressionInfoType.MemberInit
        });
      }
    }
  }
}