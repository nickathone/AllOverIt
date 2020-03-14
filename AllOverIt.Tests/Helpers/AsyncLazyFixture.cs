﻿using AllOverIt.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Helpers
{
  public class AsyncLazyFixture : AllOverItFixtureBase
  {
    public class Constructor_Type : AsyncLazyFixture
    {
      [Fact]
      public void Should_Throw_When_Null_Factory()
      {
        var actual = Awaiting(async () =>
        {
          Func<int> factory = null;

          var lazy = new AsyncLazy<int>(factory);

          await lazy;
        });

        actual.Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("function"));
      }

      [Fact]
      public async Task Should_Await_Lazy_Value()
      {
        var expected = CreateExcluding(0);

        var lazy = new AsyncLazy<int>(() => expected);

        var actual = await lazy.Value;

        actual.Should().Be(expected);
      }

      [Fact]
      public async Task Should_Await_Lazy()
      {
        var expected = CreateExcluding(0);

        var lazy = new AsyncLazy<int>(() => expected);

        var actual = await lazy;

        actual.Should().Be(expected);
      }
    }

    public class Constructor_Task_Type : AsyncLazyFixture
    {
      [Fact]
      public void Should_Throw_When_Null_Factory()
      {
        var actual = Awaiting(async () =>
        {
          Func<IList<int>> factory = null;

          var lazy = new AsyncLazy<IList<int>>(factory);

          await lazy;
        });

        actual.Should()
          .Throw<ArgumentNullException>()
          .WithMessage(GetExpectedArgumentNullExceptionMessage("function"));
      }

      [Fact]
      public async Task Should_Await_Lazy_Value()
      {
        var expected = CreateMany<int>();

        var lazy = new AsyncLazy<IReadOnlyList<int>>(() => expected);

        var actual = await lazy.Value;

        actual.Should().BeSameAs(expected);
      }

      [Fact]
      public async Task Should_Await_Lazy()
      {
        var expected = CreateMany<int>();

        var lazy = new AsyncLazy<IReadOnlyList<int>>(() => expected);

        var actual = await lazy;

        actual.Should().BeSameAs(expected);
      }
    }
  }
}