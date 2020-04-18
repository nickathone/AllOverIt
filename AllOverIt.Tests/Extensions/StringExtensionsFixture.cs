using FluentAssertions;
using System;
using System.Globalization;
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
      public void Should_Throw_When_No_Suitable_Type_Converter()
      {
        var value = Create<string>();

        Invoking(
            () =>AllOverIt.Extensions.StringExtensions.As(value, Create<int>()))
          .Should()
          .Throw<ArgumentException>()
          .WithMessage($"No converter exists for type 'Int32' when value = '{value}'.");
      }

      [Fact]
      public void Should_Return_The_Converted_Int_Value()
      {
        var value = Create<int>();
        
        var actual = AllOverIt.Extensions.StringExtensions.As($"{value}", Create<int>());

        actual.Should().Be(value);
      }

      [Fact]
      public void Should_Return_The_Converted_Double_Value()
      {
        var value = Create<double>();

        var actual = AllOverIt.Extensions.StringExtensions.As($"{value}", Create<double>());

        actual.Should().Be(value);
      }

      [Theory]
      [InlineData("1", true)]
      [InlineData("0", false)]
      [InlineData("true", true)]
      [InlineData("false", false)]
      [InlineData("True", true)]
      [InlineData("False", false)]
      [InlineData("TRUE", true)]
      [InlineData("FALSE", false)]
      public void Should_Return_The_Converted_Boolean_Value(string value, bool expected)
      {
        var actual = AllOverIt.Extensions.StringExtensions.As(value, Create<bool>());

        actual.Should().Be(expected);
      }

      [Theory]
      [InlineData(null, DummyEnum.Dummy2, DummyEnum.Dummy2)]
      [InlineData("", DummyEnum.Dummy2, DummyEnum.Dummy2)]
      [InlineData(" ", DummyEnum.Dummy2, DummyEnum.Dummy2)]
      [InlineData("Dummy2", DummyEnum.Dummy1, DummyEnum.Dummy2)]
      [InlineData("dummy2", DummyEnum.Dummy3, DummyEnum.Dummy2)]
      [InlineData("DUMMY2", DummyEnum.Dummy3, DummyEnum.Dummy2)]
      public void Should_Convert_To_Enum(string value, DummyEnum defaultValue, DummyEnum expected)
      {
        var actual = AllOverIt.Extensions.StringExtensions.As<DummyEnum>(value, defaultValue);

        actual.Should().Be(expected);
      }

      [Theory]
      [InlineData("0", NumberStyles.None)]
      [InlineData("none", NumberStyles.None)]
      [InlineData("None", NumberStyles.None)]
      [InlineData("NONE", NumberStyles.None)]

      [InlineData("1", NumberStyles.AllowLeadingWhite)]
      [InlineData("allowleadingwhite", NumberStyles.AllowLeadingWhite)]
      [InlineData("AllowLeadingWhite", NumberStyles.AllowLeadingWhite)]
      [InlineData("ALLOWLEADINGWHITE", NumberStyles.AllowLeadingWhite)]

      [InlineData("7", NumberStyles.Integer)]
      [InlineData("integer", NumberStyles.Integer)]
      [InlineData("Integer", NumberStyles.Integer)]
      [InlineData("INTEGER", NumberStyles.Integer)]
      public void Should_Convert_Enum_With_Flags_Attribute(string value, NumberStyles expected)
      {
        var actual = AllOverIt.Extensions.StringExtensions.As<NumberStyles>(value);

        actual.Should().Be(expected);
      }

      [Theory]
      [InlineData("0", DummyEnum.Dummy1)]
      [InlineData("1", DummyEnum.Dummy2)]
      [InlineData("2", DummyEnum.Dummy3)]
      public void Should_Convert_From_Number_String_To_Enum(string value, DummyEnum expected)
      {
        var actual = AllOverIt.Extensions.StringExtensions.As<DummyEnum>(value);

        actual.Should().Be(expected);
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
      public void Should_Return_The_Converted_Double_Value()
      {
        var value = Create<double>();

        var actual = AllOverIt.Extensions.StringExtensions.AsNullable<double>($"{value}");

        actual.Should().Be(value);
      }

      [Theory]
      [InlineData("true", true)]
      [InlineData("false", false)]
      [InlineData("True", true)]
      [InlineData("False", false)]
      [InlineData("TRUE", true)]
      [InlineData("FALSE", false)]
      public void Should_Return_The_Converted_Boolean_Value(string value, bool expected)
      {
        var actual = AllOverIt.Extensions.StringExtensions.AsNullable<bool>($"{value}");

        actual.Should().Be(expected);
      }

      [Theory]
      [InlineData("0", DummyEnum.Dummy1)]
      [InlineData("1", DummyEnum.Dummy2)]
      [InlineData("2", DummyEnum.Dummy3)]
      public void Should_Convert_From_Number_String_To_Nullable_Enum(string value, DummyEnum? expected)
      {
        var actual = AllOverIt.Extensions.StringExtensions.AsNullable<DummyEnum>($"{value}");

        actual.Should().Be(expected);
      }

      [Theory]
      [InlineData("dummy1", DummyEnum.Dummy1)]
      [InlineData("Dummy2", DummyEnum.Dummy2)]
      [InlineData("DUMMY3", DummyEnum.Dummy3)]
      public void Should_Return_Converted_Enum_Value(string value, DummyEnum? expected)
      {
        var actual = AllOverIt.Extensions.StringExtensions.AsNullable<DummyEnum>(value);

        actual.Should().Be(expected);
      }
    }
  }
}