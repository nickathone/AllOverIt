using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public class ComparerExtensionsFixture : FixtureBase
    {
        private sealed class StringDummy
        {
            public string First { get; init; }
            public string Second { get; init; }
        }

        private sealed class FirstDummyComparer : IComparer<StringDummy>
        {
            public static readonly IComparer<StringDummy> Instance = new FirstDummyComparer();

            public int Compare(StringDummy lhs, StringDummy rhs)
            {
                return string.Compare(lhs.First, rhs.First);
            }
        }

        private sealed class SecondDummyComparer : IComparer<StringDummy>
        {
            public static readonly IComparer<StringDummy> Instance = new SecondDummyComparer();

            public int Compare(StringDummy lhs, StringDummy rhs)
            {
                return string.Compare(lhs.Second, rhs.Second);
            }
        }

        public class Reverse : ComparerExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Comparer_Null()
            {
                Invoking(() =>
                {
                    _ = ComparerExtensions.Reverse<string>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("comparer");
            }

            [Fact]
            public void Should_Reverse_Items()
            {
                var values = CreateMany<StringDummy>().ToList();

                values.Sort(FirstDummyComparer.Instance.Reverse());

                values.Select(item => item.First).Should().BeInDescendingOrder();
            }
        }

        public class Then : ComparerExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_First_Comparer_Null()
            {
                Invoking(() =>
                {
                    _ = ComparerExtensions.Then<StringDummy>(null, SecondDummyComparer.Instance);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("first");
            }

            [Fact]
            public void Should_Throw_When_Next_Comparer_Null()
            {
                Invoking(() =>
                {
                    _ = ComparerExtensions.Then<StringDummy>(FirstDummyComparer.Instance, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("next");
            }

            [Fact]
            public void Should_Compose_Comparers()
            {
                var values = new List<StringDummy>
                {
                   new StringDummy{ First = "a", Second = Create<string>() },
                   new StringDummy{ First = "b", Second = Create<string>() },
                   new StringDummy{ First = "a", Second = Create<string>() },
                   new StringDummy{ First = "b", Second = Create<string>() },
                   new StringDummy{ First = "a", Second = Create<string>() },
                   new StringDummy{ First = "b", Second = Create<string>() },
                   new StringDummy{ First = "a", Second = Create<string>() },
                };

                var expected = values.OrderBy(item => item.First).ThenBy(item => item.Second).ToList();

                var sorter = FirstDummyComparer.Instance.Then(SecondDummyComparer.Instance);

                values.Sort(sorter);

                values.Should().ContainInOrder(expected);
            }
        }
    }
}