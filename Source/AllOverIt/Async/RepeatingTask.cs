using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Async
{
    /// <summary>Provides support for creating and executing a new task repeatedly until cancelled.</summary>
    public static class RepeatingTask
    {
        /// <summary>Creates and starts a new task that repeatedly invokes an asynchronous function.</summary>
        /// <param name="action">The asynchronous function to execute each time the task repeats.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <param name="repeatDelay">The frequency (milliseconds) the task should repeat.</param>
        /// <param name="initialDelay">An initial delay (milliseconds) to wait before executing the first function invocation.</param>
        /// <returns>The started task.</returns>
        public static Task Start(Func<Task> action, int repeatDelay, CancellationToken cancellationToken)
        {
            return StartImpl(action, 0, repeatDelay, cancellationToken);
        }

        /// <summary>Creates and starts a new task after an initial delay that repeatedly invokes an asynchronous function.</summary>
        /// <param name="action">The asynchronous function to execute each time the task repeats.</param>
        /// <param name="initialDelay">An initial delay (milliseconds) to wait before executing the first function invocation.</param>
        /// <param name="repeatDelay">The frequency (milliseconds) the task should repeat.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <returns>The started task.</returns>
        public static Task Start(Func<Task> action, int initialDelay, int repeatDelay, CancellationToken cancellationToken)
        {
            return StartImpl(action, initialDelay, repeatDelay, cancellationToken);
        }

        /// <summary>Creates and starts a new task that repeatedly invokes a function.</summary>
        /// <param name="action">The function to execute each time the task repeats.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <param name="repeatDelay">The frequency (milliseconds) the task should repeat.</param>
        /// <param name="initialDelay">An initial delay (milliseconds) to wait before executing the first function invocation.</param>
        /// <returns>The started task.</returns>
        public static Task Start(Action action, int repeatDelay, CancellationToken cancellationToken)
        {
            return StartImpl(action, 0, repeatDelay, cancellationToken);
        }

        /// <summary>Creates and starts a new task after an initial delay that repeatedly invokes a function.</summary>
        /// <param name="action">The function to execute each time the task repeats.</param>
        /// <param name="initialDelay">An initial delay (milliseconds) to wait before executing the first function invocation.</param>
        /// <param name="repeatDelay">The frequency (milliseconds) the task should repeat.</param>
        /// <param name="cancellationToken">The cancellation token used to terminate the task.</param>
        /// <returns>The started task.</returns>
        public static Task Start(Action action, int initialDelay, int repeatDelay, CancellationToken cancellationToken)
        {
            return StartImpl(action, initialDelay, repeatDelay, cancellationToken);
        }

        private static Task StartImpl(Func<Task> action, int initialDelay, int repeatDelay, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () =>
            {
                // There's no ConfigureAwait() false here as there's no SynchronizationContext to be captured

                try
                {
                    if (initialDelay > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        await Task.Delay(initialDelay, cancellationToken);
                    }

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await action.Invoke();

                        cancellationToken.ThrowIfCancellationRequested();

                        await Task.Delay(repeatDelay, cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    // break out
                }
            }, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();
        }

        private static Task StartImpl(Action action, int initialDelay, int repeatDelay, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () =>
            {
                // There's no ConfigureAwait() false here as there's no SynchronizationContext to be captured

                try
                {
                    if (initialDelay > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        await Task.Delay(initialDelay, cancellationToken);
                    }

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        action.Invoke();

                        cancellationToken.ThrowIfCancellationRequested();

                        await Task.Delay(repeatDelay, cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    // break out
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }
    }
}
