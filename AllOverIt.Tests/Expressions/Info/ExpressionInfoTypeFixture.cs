using AllOverIt.Expressions.Info;
using AllOverIt.Fixture;
using AllOverIt.Helpers;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Expressions.Info
{
  public class ExpressionInfoTypeFixture : AoiFixtureBase
  {
    public class Values : ExpressionInfoTypeFixture
    {
      [Fact]
      public void Should_Have_Available_Values()
      {
        var enumValues = EnumHelper.GetEnumValues<ExpressionInfoType>();

        enumValues.Should().BeEquivalentTo(ExpressionInfoType.Constant, ExpressionInfoType.Field, ExpressionInfoType.Property,
          ExpressionInfoType.BinaryComparison, ExpressionInfoType.MethodCall, ExpressionInfoType.Conditional, ExpressionInfoType.MemberInit,
          ExpressionInfoType.New, ExpressionInfoType.Parameter);
      }

      [Fact]
      public void Should_Have_Known_Item_Count()
      {
        var actual = EnumHelper.GetEnumValues<ExpressionInfoType>().Count;

        var expected = new[]
        {
          ExpressionInfoType.Constant, ExpressionInfoType.Field, ExpressionInfoType.Property, ExpressionInfoType.BinaryComparison,
          ExpressionInfoType.MethodCall, ExpressionInfoType.Conditional, ExpressionInfoType.MemberInit, ExpressionInfoType.New,
          ExpressionInfoType.Parameter
        }.Length;

        // if this test fails then update 'Should_Have_Expected_Values'
        actual.Should().Be(expected);
      }

      [Theory]
      [InlineData(ExpressionInfoType.Constant, 1)]
      [InlineData(ExpressionInfoType.Field, 2)]
      [InlineData(ExpressionInfoType.Property, 4)]
      [InlineData(ExpressionInfoType.BinaryComparison, 8)]
      [InlineData(ExpressionInfoType.MethodCall, 16)]
      [InlineData(ExpressionInfoType.Conditional, 32)]
      [InlineData(ExpressionInfoType.MemberInit, 64)]
      [InlineData(ExpressionInfoType.New, 128)]
      public void Should_Have_Expected_Values(ExpressionInfoType infoType, int expectedValue)
      {
        infoType.Should().BeEquivalentTo(expectedValue);
      }
    }
  }
}