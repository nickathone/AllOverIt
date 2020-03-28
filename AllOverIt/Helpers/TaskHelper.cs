using AllOverIt.Exceptions;
using System;
using System.Threading.Tasks;

namespace AllOverIt.Helpers
{
  public static class TaskHelper
  {
    private static Action<Exception> _defaultExceptionHandler;

    public static void SetDefaultExceptionHandler(Action<Exception> exceptionHandler) => _defaultExceptionHandler = exceptionHandler;

    // assumes the default handler has been configured; if it hasn't then you're on your own
    public static void SafeFireAndForget(this Task task, bool continueOnCapturedContext = true)
      => DoSafeFireAndForget(task, continueOnCapturedContext, null);

    public static void SafeFireAndForget(this Task task, Action<Exception> exceptionHandler, bool continueOnCapturedContext = true)
      => DoSafeFireAndForget(task, continueOnCapturedContext, exceptionHandler);

    public static void SafeFireAndForget(this Task task, IExceptionHandler exceptionHandler, bool continueOnCapturedContext = true)
      => DoSafeFireAndForget(task, continueOnCapturedContext, exceptionHandler.Handle);

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