using AllOverIt.Expressions.Info;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace AllOverIt.Tests.Expressions.Info
{
  public class MethodCallExpressionInfoFixture : AllOverItFixtureBase
  {
    public class Constructor : MethodCallExpressionInfoFixture
    {
      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(
            () => new MethodCallExpressionInfo(null, A.Fake<IExpressionInfo>(), A.Fake<MethodInfo>(), A.CollectionOfFake<IExpressionInfo>(5), Create<bool>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Throw_When_Object_Null()
      {
        var expression = A.Fake<Expression>();

        Invoking(
            () => new MethodCallExpressionInfo(expression, null, A.Fake<MethodInfo>(), A.CollectionOfFake<IExpressionInfo>(5), Create<bool>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("object"));
      }

      [Fact]
      public void Should_Throw_When_MethodInfo_Null()
      {
        var expression = A.Fake<Expression>();

        Invoking(
            () => new MethodCallExpressionInfo(expression, A.Fake<IExpressionInfo>(), null, A.CollectionOfFake<IExpressionInfo>(5), Create<bool>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("methodInfo"));
      }

      [Fact]
      public void Should_Throw_When_Parameters_Null()
      {
        var expression = A.Fake<Expression>();

        Invoking(
            () => new MethodCallExpressionInfo(expression, A.Fake<IExpressionInfo>(), A.Fake<MethodInfo>(), null, Create<bool>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("parameters"));
      }

      [Fact]
      public void Should_Initialize()
      {
        var expression = A.Fake<Expression>();
        var @object = A.Fake<IExpressionInfo>();
        var methodInfo = A.Fake<MethodInfo>();
        var parameters = A.CollectionOfFake<IExpressionInfo>(5);
        var isNegated = Create<bool>();

        var actual = new MethodCallExpressionInfo(expression, @object, methodInfo, parameters, isNegated);

        actual.Should().BeEquivalentTo(new
        {
          Expression = expression,
          Object = @object,
          MethodInfo = methodInfo,
          Parameters = parameters,
          InfoType = ExpressionInfoType.MethodCall
        });
      }
    }
  }
}