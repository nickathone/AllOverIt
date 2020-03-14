using AllOverIt.Extensions;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
  // SetDefaultExceptionHandler() updates a static - tests need to run sequentially
  [CollectionDefinition(nameof(DefaultExceptionHandlerCollection), DisableParallelization = true)]
  public class DefaultExceptionHandlerCollection { }

  public class TaskExFixture : AllOverItFixtureBase
  {
    [Collection(nameof(DefaultExceptionHandlerCollection))]
    public class SetDefaultExceptionHandler : TaskExFixture
    {
      [Fact(Timeout = 5000)]
      public async Task Should_Execute_Default_Handler()
      {
        var tcs = new TaskCompletionSource<bool>();
        Exception handledException = null;

        TaskEx.SetDefaultExceptionHandler(exception =>
        {
          handledException = exception;

          Task.Run(() => tcs.TrySetResult(true));
        });

        var exception = new ArgumentException();

        TaskEx.SafeFireAndForget(DelayThenThrow(exception));

        await tcs.Task.ConfigureAwait(false);

        handledException.Should().BeSameAs(exception);
      }
    }

    [Collection(nameof(DefaultExceptionHandlerCollection))]
    public class SafeFireAndForget : TaskExFixture
    {
      [Fact(Timeout = 5000)]
      public async Task Should_Execute_Handler()
      {
        var tcs = new TaskCompletionSource<bool>();
        Exception handledException = null;

        var exception = new ArgumentException();

        TaskEx.SafeFireAndForget(DelayThenThrow(exception), true, ex =>
        {
          handledException = ex;

          Task.Run(() => tcs.TrySetResult(true));
        });

        await tcs.Task.ConfigureAwait(false);

        handledException.Should().BeSameAs(exception);
      }

      [Fact(Timeout = 5000)]
      public async Task Should_Execute_Default_And_Local_Handler()
      {
        var tcs1 = new TaskCompletionSource<bool>();
        var tcs2 = new TaskCompletionSource<bool>();
        Exception handledException1 = null;
        Exception handledException2 = null;

        TaskEx.SetDefaultExceptionHandler(ex =>
        {
          handledException1 = ex;
          tcs1.TrySetResult(true);
        });

        var exception = new ArgumentException();

        TaskEx.SafeFireAndForget(DelayThenThrow(exception), true, ex =>
        {
          handledException2 = ex;
          tcs2.TrySetResult(true);
        });

        await Task.WhenAll(tcs1.Task, tcs2.Task).ConfigureAwait(false);

        handledException1.Should().BeSameAs(exception);
        handledException1.Should().BeSameAs(handledException2);
      }
    }

    public class WhenAll : TaskExFixture
    {
      [Fact]
      public async Task Should_Wait_For_Two_Tasks()
      {
        var value1 = Create<bool>();
        var value2 = Create<int>();

        var task1 = Task.FromResult(value1);
        var task2 = Task.FromResult(value2);

        var actual = await TaskEx.WhenAll(task1, task2).ConfigureAwait(false);

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

        var actual = await TaskEx.WhenAll(task1, task2, task3).ConfigureAwait(false);

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

        var actual = await TaskEx.WhenAll(task1, task2, task3, task4).ConfigureAwait(false);

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

        var actual = await TaskEx.WhenAll(task1, task2, task3, task4, task5).ConfigureAwait(false);

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

        var actual = await TaskEx.WhenAll(
          TaskEx.WhenAll(task1, task2, task3),
          TaskEx.WhenAll(task4, task5, task6)
        ).ConfigureAwait(false);

        var group1 = actual.Item1;
        var group2 = actual.Item2;

        actual
          .Should()
          .Be((group1, group2));
      }
    }

    private static async Task DelayThenThrow(ArgumentException exception)
    {
      await Task.Delay(10).ConfigureAwait(false);
      throw exception;
    }
  }
}