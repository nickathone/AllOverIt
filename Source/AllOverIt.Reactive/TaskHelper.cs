using AllOverIt.Assertion;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AllOverIt.Reactive
{
    /// <summary>Provides a variety of helpe methods for <see cref="Task"/> and <see cref="Task{T}"/>.</summary>
    public static class TaskHelper
    {
        /// <summary>Asynchronously invokes the provided <paramref name="action"/> while ensuring exceptions are re-raised.
        /// The method does not return until the action completes.</summary>
        /// <param name="action">The asynchronously action to invoke.</param>
        public static void ExecuteAsyncAndWait(Func<Task> action)
        {
            _ = action.WhenNotNull(nameof(action));

            Observable
               .FromAsync(action.Invoke)
               .Subscribe();
        }

        /// <summary>Asynchronously invokes the provided <paramref name="action"/> while ensuring exceptions are re-raised.
        /// The method does not return until the action completes.</summary>
        /// <typeparam name="TResult">The result type returned from the action.</typeparam>
        /// <param name="action">The asynchronously action to invoke.</param>
        /// <returns>The result of the invoked action.</returns>
        public static TResult ExecuteAsyncAndWait<TResult>(Func<Task<TResult>> action)
        {
            _ = action.WhenNotNull(nameof(action));

            TResult returnValue = default;

            Observable
               .FromAsync(action.Invoke)
               .Subscribe(result => returnValue = result);
            
            return returnValue;
        }
    }
}
