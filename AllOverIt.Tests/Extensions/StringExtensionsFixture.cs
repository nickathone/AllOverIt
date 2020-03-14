using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
  public class StringExtensionsFixture : AllOverItFixtureBase
  {
    public enum DummyEnum : short
    {
      Dummy1, Dummy2, Dummy3
    }

    public class As : StringExtensionsFixture
    {
      [Fact]
      public void Should_Return_Default_Value_If_Value_Is_Null()
      {
        var defaultValue = Create<int>();

        var actual = AllOverIt.Extensions.StringExtensions.As(null, defaultValue);

        actual.Should().Be(defaultValue);
      }

      [Fact]
      public void Should_Return_Default_Value_If_Value_Is_Whitespace()
      {
        var defaultValue = Create<int>();

        var actual = AllOverIt.Extensions.StringExtensions.As("  ", defaultValue);

        actual.Should().Be(defaultValue);
      }

      [Fact]
      public void Should_Return_Default_Value_If_Value_Is_Empty()
      {
        var defaultValue = Create<int>();

        var actual = AllOverIt.Extensions.StringExtensions.As(string.Empty, defaultValue);

        actual.Should().Be(defaultValue);
      }

      [Fact]
      public void Should_Return_Default_Value_If_Value_Is_Not_Convertible()
      {
        var defaultValue = Create<int>();
        
        var actual = AllOverIt.Extensions.StringExtensions.As(Create<string>(), defaultValue);

        actual.Should().Be(defaultValue);
      }

      [Fact]
      public void Should_Return_The_Converted_Value()
      {
        var defaultValue = Create<int>();
        var value = Create<int>();
        
        var actual = AllOverIt.Extensions.StringExtensions.As($"{value}", defaultValue);

        actual.Should().Be(value);
      }

      [Theory]
      [InlineData(null, true, DummyEnum.Dummy2, DummyEnum.Dummy2)]
      [InlineData(null, false, DummyEnum.Dummy2, DummyEnum.Dummy2)]
      [InlineData("", true, DummyEnum.Dummy2, DummyEnum.Dummy2)]
      [InlineData("", false, DummyEnum.Dummy2, DummyEnum.Dummy2)]
      [InlineData(" ", true, DummyEnum.Dummy2, DummyEnum.Dummy2)]
      [InlineData(" ", false, DummyEnum.Dummy2, DummyEnum.Dummy2)]
      [InlineData("Dummy2", false, DummyEnum.Dummy1, DummyEnum.Dummy2)]
      [InlineData("Dummy2", true, DummyEnum.Dummy1, DummyEnum.Dummy2)]
      [InlineData("dummy2", true, DummyEnum.Dummy3, DummyEnum.Dummy2)]
      public void Should_Convert_To_Enum(string value, bool ignoreCase, DummyEnum defaultValue, DummyEnum expected)
      {
        var actual = AllOverIt.Extensions.StringExtensions.As<DummyEnum>(value, defaultValue, ignoreCase);

        actual.Should().Be(expected);
      }

      [Fact]
      public void Should_Fail_To_Convert_To_Enum()
      {
        Invoking(
            () => AllOverIt.Extensions.StringExtensions.As<DummyEnum>("dummy2", DummyEnum.Dummy3, false))
          .Should()
          .Throw<ArgumentException>()
          .WithMessage("Requested value 'dummy2' was not found.");
      }
    }

    public class AsNullable : StringExtensionsFixture
    {
      [Fact]
      public void Should_Return_Null_If_Value_Is_Null()
      {
        var actual = AllOverIt.Extensions.StringExtensions.AsNullable<int>(null);

        actual.Should().NotHaveValue();
      }

      [Fact]
      public void Should_Return_Default_Value_If_Value_Is_Whitespace()
      {
        var actual = AllOverIt.Extensions.StringExtensions.AsNullable<int>("  ");

        actual.Should().NotHaveValue();
      }

      [Fact]
      public void Should_Return_Default_Value_If_Value_Is_Empty()
      {
        var actual = AllOverIt.Extensions.StringExtensions.AsNullable<int>(string.Empty);

        actual.Should().NotHaveValue();
      }

      [Fact]
      public void Should_Return_Default_Value_If_Value_Is_Not_Convertible()
      {
        var actual = AllOverIt.Extensions.StringExtensions.AsNullable<int>(Create<string>());

        actual.Should().NotHaveValue();
      }

      [Fact]
      public void Should_Return_Converted_Int_Value()
      {
        var value = Create<int>();

        var actual = AllOverIt.Extensions.StringExtensions.AsNullable<int>($"{value}");

        actual.Should().Be(value);
      }

      [Fact]
      public void Should_Return_Converted_Enum_Value()
      {
        var value = Create<DummyEnum>();

        var actual = AllOverIt.Extensions.StringExtensions.AsNullable<DummyEnum>($"{value}");

        actual.Should().Be(value);
      }

      [Fact]
      public void Should_Return_Converted_Enum_Value_IgnoreCase()
      {
        var value = Create<DummyEnum>();

        var actual = AllOverIt.Extensions.StringExtensions.AsNullable<DummyEnum>($"{value}".ToLower(), true);

        actual.Should().Be(value);
      }

      [Fact]
      public void Should_Fail_To_Convert_To_Enum()
      {
        Invoking(
            () => AllOverIt.Extensions.StringExtensions.AsNullable<DummyEnum>("dummy2", false))
          .Should()
          .Throw<ArgumentException>()
          .WithMessage("Requested value 'dummy2' was not found.");
      }
    }
  }
}