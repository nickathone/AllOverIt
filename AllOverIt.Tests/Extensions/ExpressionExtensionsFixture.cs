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
      public void Should_Get_Constant_Expression_Value()
      {
        Expression<Func<int>> expression = () => 10;

        var actual = expression.Body.GetValue();

        actual.Should().Be(10);
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
    }
  }
}