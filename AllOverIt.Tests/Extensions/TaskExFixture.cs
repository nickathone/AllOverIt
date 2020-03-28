using AllOverIt.Extensions;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
  public class TaskExFixture : AllOverItFixtureBase
  {
    public class WhenAll : TaskExFixture
    {
      [Fact]
      public async Task Should_Wait_For_Two_Tasks()
      {
        var value1 = Create<bool>();
        var value2 = Create<int>();

        var task1 = Task.FromResult(value1);
        var task2 = Task.FromResult(value2);

        var actual = await TaskUtils.WhenAll(task1, task2).ConfigureAwait(false);

        actual.Should().Be((value1, value2));
      }

      [Fact]
      public async Task Should_Wait_For_Three_Tasks()
      {
        var value1 = Create<bool>();
        var value2 = Create<int>();
        var value3 = Create<string>();

        var task1 = Task.FromResult(value1);
        var task2 = Task.FromResult(value2);
        var task3 = Task.FromResult(value3);

        var actual = await TaskUtils.WhenAll(task1, task2, task3).ConfigureAwait(false);

        actual.Should().Be((value1, value2, value3));
      }

      [Fact]
      public async Task Should_Wait_For_For_Tasks()
      {
        var value1 = Create<bool>();
        var value2 = Create<int>();
        var value3 = Create<string>();
        var value4 = Create<double>();

        var task1 = Task.FromResult(value1);
        var task2 = Task.FromResult(value2);
        var task3 = Task.FromResult(value3);
        var task4 = Task.FromResult(value4);

        var actual = await TaskUtils.WhenAll(task1, task2, task3, task4).ConfigureAwait(false);

        actual.Should().Be((value1, value2, value3, value4));
      }

      [Fact]
      public async Task Should_Wait_For_Five_Tasks()
      {
        var value1 = Create<bool>();
        var value2 = Create<int>();
        var value3 = Create<string>();
        var value4 = Create<double>();
        var value5 = Create<short>();

        var task1 = Task.FromResult(value1);
        var task2 = Task.FromResult(value2);
        var task3 = Task.FromResult(value3);
        var task4 = Task.FromResult(value4);
        var task5 = Task.FromResult(value5);

        var actual = await TaskUtils.WhenAll(task1, task2, task3, task4, task5).ConfigureAwait(false);

        actual.Should().Be((value1, value2, value3, value4, value5));
      }

      [Fact]
      public async Task Should_Wait_For_Composed_Tasks()
      {
        var value1 = Create<bool>();
        var value2 = Create<int>();
        var value3 = Create<string>();
        var value4 = Create<double>();
        var value5 = Create<short>();
        var value6 = Create<int>();

        var task1 = Task.FromResult(value1);
        var task2 = Task.FromResult(value2);
        var task3 = Task.FromResult(value3);
        var task4 = Task.FromResult(value4);
        var task5 = Task.FromResult(value5);
        var task6 = Task.FromResult(value6);

        var actual = await TaskUtils.WhenAll(
          TaskUtils.WhenAll(task1, task2, task3),
          TaskUtils.WhenAll(task4, task5, task6)
        ).ConfigureAwait(false);

        var group1 = actual.Item1;
        var group2 = actual.Item2;

        actual
          .Should()
          .Be((group1, group2));
      }
    }
  }
}