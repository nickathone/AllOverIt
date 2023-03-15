using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace AllOverIt.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class TaskExtensions
    {
        public static void FireAndForget(this Task task, Action<Exception> exceptionHandler = null)
        {
            _ = DoFireAndForget(task, exceptionHandler);
        }

        public static void FireAndForget<TException>(this Task task, Action<TException> exceptionHandler = null)
            where TException : Exception
        {
            _ = DoFireAndForget(task, exceptionHandler);
        }

        private static async Task DoFireAndForget<TException>(Task task, Action<TException> exceptionHandler)
            where TException : Exception
        {
            if (!task.IsCompleted || task.IsFaulted)
            {
                try
                {
                    // No need to resume on the original SynchronizationContext
                    await task.ConfigureAwait(false);
                }
                catch (TException ex) when (exceptionHandler is not null)
                {
                    exceptionHandler?.Invoke(ex);
                }
            }
        }
    }
}