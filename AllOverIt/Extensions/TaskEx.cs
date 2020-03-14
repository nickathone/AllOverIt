using System;
using System.Threading.Tasks;

namespace AllOverIt.Extensions
{
  // not using TaskExtensions as it will conflict with System.Threading.Tasks.TaskExtensions
  public static class TaskEx
  {
    private static Action<Exception> _defaultExceptionHandler;

    public static void SetDefaultExceptionHandler(Action<Exception> exceptionHandler) => _defaultExceptionHandler = exceptionHandler;

    public static void SafeFireAndForget(this Task task, bool continueOnCapturedContext = true, Action<Exception> exceptionHandler = null)
       => DoSafeFireAndForget(task, continueOnCapturedContext, exceptionHandler);

    public static async Task<(T1, T2)> WhenAll<T1, T2>(Task<T1> task1, Task<T2> task2)
    {
      // await them all to ensure exceptions are handled correctly
      await Task.WhenAll(task1, task2).ConfigureAwait(false);

      return (task1.Result, task2.Result);
    }

    public static async Task<(T1, T2, T3)> WhenAll<T1, T2, T3>(Task<T1> task1, Task<T2> task2, Task<T3> task3)
    {
      // await them all to ensure exceptions are handled correctly
      await Task.WhenAll(task1, task2, task3).ConfigureAwait(false);

      return (task1.Result, task2.Result, task3.Result);
    }

    public static async Task<(T1, T2, T3, T4)> WhenAll<T1, T2, T3, T4>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4)
    {
      // await them all to ensure exceptions are handled correctly
      await Task.WhenAll(task1, task2, task3, task4).ConfigureAwait(false);

      return (task1.Result, task2.Result, task3.Result, task4.Result);
    }

    public static async Task<(T1, T2, T3, T4, T5)> WhenAll<T1, T2, T3, T4, T5>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4, Task<T5> task5)
    {
      // await them all to ensure exceptions are handled correctly
      await Task.WhenAll(task1, task2, task3, task4, task5).ConfigureAwait(false);

      return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result);
    }

    private static async void DoSafeFireAndForget(Task task, bool continueOnCapturedContext, Action<Exception> exceptionHandler)
    {
      try
      {
        await task.ConfigureAwait(continueOnCapturedContext);
      }
      catch (Exception ex) when (_defaultExceptionHandler != null || exceptionHandler != null)
      {
        _defaultExceptionHandler?.Invoke(ex);
        exceptionHandler?.Invoke(ex);
      }
    }
  }
}