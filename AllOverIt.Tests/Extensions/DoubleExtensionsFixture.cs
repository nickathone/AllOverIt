using AllOverIt.Extensions;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
  public class DoubleExtensionsFixture : AllOverItFixtureBase
  {
    public class IsZero : DoubleExtensionsFixture
    {
      [Fact]
      public void Should_Compare_To_Zero()
      {
        var actual = DoubleExtensions.IsZero(0.0d);

        actual.Should().BeTrue();
      }

      [Fact]
      public void Should_Not_Compare_To_Internal_Epsilon()
      {
        var actual = DoubleExtensions.IsZero(1E-07);

        actual.Should().BeFalse();
      }

      [Fact]
      public void Should_Compare_To_Less_Than_Internal_Epsilon()
      {
        var value = 1E-08;
        var actual = DoubleExtensions.IsZero(value);

        actual.Should().BeTrue();
      }

      [Fact]
      public void Should_Not_Compare_To_Greater_Than_Internal_Epsilon()
      {
        var value = 1E-06;
        var actual = DoubleExtensions.IsZero(value);

        actual.Should().BeFalse();
      }

      [Fact]
      public void Should_Not_Compare_Negative_To_Zero()
      {
        var value = -Create<double>() - 0.1d;
        var actual = DoubleExtensions.IsZero(value);

        actual.Should().BeFalse();
      }

      [Fact]
      public void Should_Not_Compare_Positive_To_Zero()
      {
        var value = -Create<double>() + 0.1d;
        var actual = DoubleExtensions.IsZero(value);

        actual.Should().BeFalse();
      }
    }

    public class IsEqualTo : DoubleExtensionsFixture
    {
      [Theory]
      [InlineData(1E-07, 0.0d)]
      [InlineData(0.0d, 1E-07)]
      public void Should_Not_Compare_To_Epsilon_Difference(double val1, double val2)
      {
        var actual = DoubleExtensions.IsEqualTo(val1, val2);

        actual.Should().BeFalse();
      }

      [Theory]
      [InlineData(1E-08, 0.0d)]
      [InlineData(0.0d, 1E-08)]
      public void Should_Compare_To_Less_Than_Epsilon(double val1, double val2)
      {
        var actual = DoubleExtensions.IsEqualTo(val1, val2);

        actual.Should().BeTrue();
      }

      [Theory]
      [InlineData(1E-06, 0.0d)]
      [InlineData(0.0d, 1E-06)]
      public void Should_Not_Compare_To_Greater_Than_Epsilon(double val1, double val2)
      {
        var actual = DoubleExtensions.IsEqualTo(val1, val2);

        actual.Should().BeFalse();
      }

      [Fact]
      public void Should_Compare_Same_Value()
      {
        var val = Create<double>();
        var actual = DoubleExtensions.IsEqualTo(val, val);

        actual.Should().BeTrue();
      }

      [Fact]
      public void Should_Not_Compare_Different_Value()
      {
        var val1 = Create<double>();
        var val2 = CreateExcluding<double>(val1);

        var actual = DoubleExtensions.IsEqualTo(val1, val2);

        actual.Should().BeFalse();
      }
    }
  }
}