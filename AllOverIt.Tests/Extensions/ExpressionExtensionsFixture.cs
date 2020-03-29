using AllOverIt.Extensions;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
  public class ExpressionExtensionsFixture : AllOverItFixtureBase
  {
    private class DummyPropertyClass
    {
      public int Value { get; set; }
    }

    public class GetValue : ExpressionExtensionsFixture
    {
      [Fact]
      public void Should_Return_Null_For_Null()
      {
        var actual = ExpressionExtensions.GetValue(null);

        actual.Should().BeNull();
      }

      [Fact]
      public void Should_Throw_For_ParameterExpression()
      {
        Expression<Action<int, int>> expression = (a, b) => GetSum(a, b);

        var actual = Invoking(() =>
        {
          expression.Body.GetValue();
        });

        actual
          .Should()
          .Throw<InvalidOperationException>()
          .WithMessage("A ParameterExpression does not have a value");
      }

      [Fact]
      public void Should_Get_Constant_Expression_Value()
      {
        Expression<Func<int>> expression = () => 10;

        var actual = expression.Body.GetValue();

        actual.Should().Be(10);
      }

      [Fact]
      public void Should_Get_Lamba_Expression_Value()
      {
        var value = Create<int>();

        Expression<Func<int>> expression = () => value;

        var actual = ExpressionExtensions.GetValue(expression);

        actual.Should().Be(value);
      }

      [Fact]
      public void Should_Get_Member_Field_Expression_Value()
      {
        var value = Create<int>();
        Expression<Func<int>> expression = () => value;

        var actual = expression.Body.GetValue();

        actual.Should().Be(value);
      }

      [Fact]
      public void Should_Get_Member_Property_Expression_Value()
      {
        var dummy = Create<DummyPropertyClass>();
        Expression<Func<int>> expression = () => dummy.Value;

        var actual = expression.Body.GetValue();

        actual.Should().Be(dummy.Value);
      }

      [Fact]
      public void Should_Get_Member_Expression_Array_Value()
      {
        var dummy = CreateMany<DummyPropertyClass>(2);
        Expression<Func<int>> expression = () => dummy[1].Value;

        var actual = expression.Body.GetValue();

        actual.Should().Be(dummy[1].Value);
      }

      [Fact]
      public void Should_Get_MethodCall_Expression_Value()
      {
        var val1 = Create<int>();
        var val2 = Create<int>();
        
        Expression<Func<int>> expression = () => GetSum(val1, val2);

        var actual = expression.Body.GetValue();
        var expected = val1 + val2;

        actual.Should().Be(expected);
      }

      [Fact]
      public void Should_Throw_Inner_TargetInvocationException()
      {
        var message = Create<string>();
        Expression<Func<int>> expression = () => GetException(message);

        var actual = Invoking(() => expression.Body.GetValue());

        actual.Should().Throw<Exception>().WithMessage(message);
      }

      [Fact]
      public void Should_Get_Dynamic_Invocation_Expression_Value()
      {
        var val1 = Create<int>();
        var val2 = Create<int>();

        Expression<Func<int[]>> expression = () => new[] {val1, val2};

        var actual = expression.Body.GetValue();
        var expected = new[] { val1, val2 };

        actual.Should().BeEquivalentTo(expected);
      }

      private static int GetSum(int val1, int val2)
      {
        return val1 + val2;
      }

      private int GetException(string message)
      {
        throw new Exception(message);
      }
    }


    public class GetValues : ExpressionExtensionsFixture
    {
      [Fact]
      public void Should_Throw_When_ExpressionsNull()
      {
        var actual = Invoking(() =>
        {
          ExpressionExtensions.GetValues(null);
        });

        actual
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("expressions"));
      }

      [Fact]
      public void Should_Get_Expression_Values()
      {
        var values = CreateMany<int>(2);
        var dummy = Create<DummyPropertyClass>();

        var expression1 = Expression.Constant(values[0]);
        Expression<Func<int>> expression2 = () => values[1];
        Expression<Func<int>> expression3 = () => dummy.Value;

        var expressions = new Expression[]
        {
          expression1,
          expression2,
          expression3
        };

        var actual = ExpressionExtensions.GetValues(expressions);

        actual.Should().BeEquivalentTo(values[0], values[1], dummy.Value);
      }
    }

    public class RemoveUnary : ExpressionExtensionsFixture
    {
      [Fact]
      public void Should_Remove_Unary_Expression()
      {
        int[] values = { 1, 2, 3 };
        Expression<Func<int>> expression = () => values.Length;

        var actual = ExpressionExtensions.RemoveUnary(expression.Body);

        var unary = expression.Body as UnaryExpression;
        var expected = (MemberExpression)unary.Operand;

        actual.Should().BeSameAs(expected);
      }

      [Fact]
      public void Should_Return_Same_Expression()
      {
        var value = Create<int>();
        Expression<Func<int>> expression = () => value;

        var actual = ExpressionExtensions.RemoveUnary(expression.Body);

        var expected = expression.Body;

        actual.Should().BeSameAs(expected);
      }
    }
  }
}