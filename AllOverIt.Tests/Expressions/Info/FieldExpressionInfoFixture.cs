using AllOverIt.Expressions.Info;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace AllOverIt.Tests.Expressions.Info
{
  public class FieldExpressionInfoFixture : AllOverItFixtureBase
  {
    public class Constructor : FieldExpressionInfoFixture
    {
      [Fact]
      public void Should_Throw_When_Expression_Null()
      {
        Invoking(
            () => new FieldExpressionInfo(null, A.Fake<FieldInfo>(), Create<string>(), Create<bool>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expression"));
      }

      [Fact]
      public void Should_Throw_When_FieldInfo_Null()
      {
        var value = Create<double>();
        Expression<Func<double>> expression = () => value;

        Invoking(
            () => new FieldExpressionInfo(expression.Body, null, Create<string>(), Create<bool>()))
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("fieldInfo"));
      }

      [Fact]
      public void Should_Initialize()
      {
        var expression = A.Fake<Expression>();
        var fieldInfo = A.Fake<FieldInfo>();
        var value = Create<double>();
        var isNegated = Create<bool>();

        var actual = new FieldExpressionInfo(expression, fieldInfo, value, isNegated);

        actual.Should().BeEquivalentTo(new
        {
          Expression = expression,
          FieldInfo = fieldInfo,
          Value = value,
          IsNegated = isNegated,
          InfoType = ExpressionInfoType.Field
        });
      }
    }
  }
}