using AllOverIt.Expressions.Info;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace AllOverIt.Tests.Expressions.Info
{
  public class PropertyExpressionInfoFixture : AllOverItFixtureBase
  {
    public class Constructor : PropertyExpressionInfoFixture
    {
      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(
            () => new PropertyExpressionInfo(null, A.Fake<PropertyInfo>(), Create<string>(), Create<bool>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Throw_When_PropertyInfo_Null()
      {
        var expression = A.Fake<Expression>();

        Invoking(
            () => new PropertyExpressionInfo(expression, null, Create<string>(), Create<bool>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("propertyInfo"));
      }

      [Fact]
      public void Should_Initialize()
      {
        var expression = A.Fake<Expression>();
        var propertyInfo = A.Fake<PropertyInfo>();
        var value = Create<string>();
        var isNegated = Create<bool>();

        var actual = new PropertyExpressionInfo(expression, propertyInfo, value, isNegated);

        actual.Should().BeEquivalentTo(new
        {
          Expression = expression,
          PropertyInfo = propertyInfo,
          Value = value,
          IsNegated = isNegated,
          InfoType = ExpressionInfoType.Property
        });
      }
    }
  }
}