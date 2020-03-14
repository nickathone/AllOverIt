using AllOverIt.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
  public class EnumerableExtensionsFixture : AllOverItFixtureBase
  {
    public class AsList : EnumerableExtensionsFixture
    {
      [Fact]
      public void Should_Throw_When_Null()
      {
        IEnumerable<object> items = null;

        Invoking(
            () => items.AsList())
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("items"));
      }

      [Fact]
      public void Should_Return_Same_List()
      {
        var expected = CreateMany<int>();

        var actual = expected.AsList();

        actual.Should().BeSameAs(expected);
      }

      [Fact]
      public void Should_Return_New_List()
      {
        var dictionary = new Dictionary<int, string>
        {
          {Create<int>(), Create<string>()},
          {Create<int>(), Create<string>()},
          {Create<int>(), Create<string>()}
        };

        var actual = dictionary.AsList();

        var sameReference = ReferenceEquals(dictionary, actual);

        sameReference.Should().BeFalse();
        actual.Should().BeOfType<List<KeyValuePair<int, string>>>();
      }
    }

    public class AsReadOnlyList : EnumerableExtensionsFixture
    {
      [Fact]
      public void Should_Throw_When_Null()
      {
        IEnumerable<object> items = null;

        Invoking(
            () => items.AsReadOnlyList())
          .Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("items"));
      }

      [Fact]
      public void Should_Return_Same_List()
      {
        var expected = CreateMany<int>();

        var actual = expected.AsReadOnlyList();

        actual.Should().BeSameAs(expected);
      }

      [Fact]
      public void Should_Return_New_List()
      {
        var dictionary = new Dictionary<int, string>
        {
          {Create<int>(), Create<string>()},
          {Create<int>(), Create<string>()},
          {Create<int>(), Create<string>()}
        };

        var actual = dictionary.AsReadOnlyList();

        var sameReference = ReferenceEquals(dictionary, actual);

        sameReference.Should().BeFalse();
        actual.Should().BeOfType<List<KeyValuePair<int, string>>>();
      }
    }
  }
}