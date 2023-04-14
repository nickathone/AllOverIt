using AllOverIt.Assertion;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AllOverIt.Wpf.Threading
{
    public static class UIThread
    {
        // Is the calling thread the UI thread
        public static bool IsBound()
        {
            return GetIsBound(out _);
        }

        // Binds the current task to the UI thread
        /// <code>
        /// // On UI or worker thread context here
        /// 
        /// await UIThread.Bind();
        /// 
        /// // On the UI thread context here
        /// </code>
        public static UIThreadAwaitable BindTo()
        {
            return new UIThreadAwaitable();
        }

        // Execute the action on the UI thread
        public static async Task InvokeAsync(Func<Task> action)
        {
            _ = action.WhenNotNull(nameof(action));

            // Switch to the UI thread using the awaitable / awaiter pattern
            await BindTo();

            // Execute the action on the UI thread - makes no difference calling ConfigureAwait(false)
            // here because there's no subsequent code execution but it's a consistent good practice.
            await action.Invoke().ConfigureAwait(false);
        }

        // Execute the action on the UI thread
        public static async Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> action)
        {
            _ = action.WhenNotNull(nameof(action));

            // Switch to the UI thread using the awaitable / awaiter pattern
            await BindTo();

            // Execute the action on the UI thread - makes no difference calling ConfigureAwait(false)
            // here because there's no subsequent code execution but it's a consistent good practice.
            return await action.Invoke().ConfigureAwait(false);
        }

        // Executes the continuation on the main thread - the bool indicates if execution is running on the UI thread.
        // Example usage would be following a call to AllOverIt.Extensions.TaskExtensions.FireAndForget() and an exception
        // callback provides the ExceptionDispatchInfo (edi). The following call will switch to the UI thread and re-raise
        // the exception: ContinueOnUIThread(_ => edi.Throw())
        public static void ContinueOnUIThread(Action<bool> continuation)
        {
            _ = continuation.WhenNotNull(nameof(continuation));

            if (GetIsBound(out var dispatcher))
            {
                continuation(true);
            }
            else
            {
                _ = dispatcher.BeginInvoke(DispatcherPriority.Normal, () => continuation.Invoke(true));
            }
        }

        public static void ContinueOnWorkerThread(Action continuation)
        {
            _ = continuation.WhenNotNull(nameof(continuation));

            if (GetIsBound(out _))
            {
                ThreadPool.QueueUserWorkItem(_ => continuation(), null);
            }
            else
            {
                continuation();
            }
        }

        private static bool GetIsBound(out Dispatcher dispatcher)
        {
            dispatcher = Application.Current?.Dispatcher;

            Throw<InvalidOperationException>.WhenNull(dispatcher, "The application's dispatcher is not available.");
            
            return dispatcher.CheckAccess();
        }
    }
}