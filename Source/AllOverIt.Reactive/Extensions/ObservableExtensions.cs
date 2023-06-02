using AllOverIt.Assertion;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace AllOverIt.Reactive.Extensions
{
    /// <summary>Provides extension methods for <see cref="IObservable{T}"/>.</summary>
    public static class ObservableExtensions
    {
        /// <summary>Subscribes to an observable and waits for a specified condition to be satisfied before returning a result.</summary>
        /// <typeparam name="TResult">The type being observed and returned from the observable.</typeparam>
        /// <param name="observable">The observable to wait for.</param>
        /// <param name="predicate">The condition to be satisfied.</param>
        /// <param name="action">An optional action to apply to the result before returning it.</param>
        /// <returns>The result of the observable.</returns>
        public static IObservable<TResult> WaitUntil<TResult>(
            this IObservable<TResult> observable,
            Func<TResult, bool> predicate,
            Func<TResult, TResult> action = default)
        {
            _ = observable.WhenNotNull(nameof(observable));
            _ = predicate.WhenNotNull(nameof(predicate));

            action ??= result => result;

            return observable.WaitUntil<TResult, TResult>(predicate, action);
        }

        /// <summary>Subscribes to an observable and waits for a specified condition to be satisfied before returning a projected result.</summary>
        /// <typeparam name="TType">The type being observed.</typeparam>
        /// <typeparam name="TResult">The projected result type of the observable.</typeparam>
        /// <param name="observable">The observable to wait for.</param>
        /// <param name="predicate">The condition to be satisfied.</param>
        /// <param name="action">An action to apply to the observable result so it can be projected to another result type.</param>
        /// <returns>The projected result of the observable.</returns>
        public static IObservable<TResult> WaitUntil<TType, TResult>(
            this IObservable<TType> observable,
            Func<TType, bool> predicate,
            Func<TType, TResult> action)
        {
            _ = observable.WhenNotNull(nameof(observable));
            _ = predicate.WhenNotNull(nameof(predicate));
            _ = action.WhenNotNull(nameof(action));

            return observable
                .TakeUntil(predicate.Invoke)
                .LastAsync()
                .Select(action.Invoke);
        }

        /// <summary>Subscribes to an observable and waits for a specified condition to be satisfied before returning a result.</summary>
        /// <typeparam name="TResult">The type being observed and returned from the observable.</typeparam>
        /// <param name="observable">The observable to wait for.</param>
        /// <param name="predicate">The condition to be satisfied.</param>
        /// <param name="action">An optional action to apply to the result before returning it.</param>
        /// <returns>The result of the observable.</returns>
        public static Task<TResult> WaitUntilAsync<TResult>(
            this IObservable<TResult> observable,
            Func<TResult, bool> predicate,
            Func<TResult, Task<TResult>> action = default)
        {
            _ = observable.WhenNotNull(nameof(observable));
            _ = predicate.WhenNotNull(nameof(predicate));

            action ??= Task.FromResult;

            return observable.WaitUntilAsync<TResult, TResult>(predicate, action);
        }

        /// <summary>Subscribes to an observable and waits for a specified condition to be satisfied before returning a projected result.</summary>
        /// <typeparam name="TType">The type being observed.</typeparam>
        /// <typeparam name="TResult">The projected result type of the observable.</typeparam>
        /// <param name="observable">The observable to wait for.</param>
        /// <param name="predicate">The condition to be satisfied.</param>
        /// <param name="action">An action to apply to the observable result so it can be projected to another result type.</param>
        /// <returns>The projected result of the observable.</returns>
        public static async Task<TResult> WaitUntilAsync<TType, TResult>(
            this IObservable<TType> observable,
            Func<TType, bool> predicate,
            Func<TType, Task<TResult>> action)
        {
            _ = observable.WhenNotNull(nameof(observable));
            _ = predicate.WhenNotNull(nameof(predicate));
            _ = action.WhenNotNull(nameof(action));

            var tcs = new TaskCompletionSource<TResult>();

            var subscription = observable
                .TakeUntil(predicate)
                .LastAsync()
                .SelectMany(action.Invoke)
                .Subscribe(
                    state =>
                    {
                        tcs.SetResult(state);
                    },
                    exception =>
                    {
                        tcs.SetException(exception);
                    },
                    () => { });

            using (subscription)
            {
                // If there was an exception it will be re-thrown as we cannot return a result
                return await tcs.Task;
            }
        }






        public static IObservable<TType> RetryOnException<TType, TException>(this IObservable<TType> observable,
            int maxRetryCount, TimeSpan delay, Action<Exception, int> onError = default, IScheduler scheduler = default)
            where TException : Exception
        {
            return RetryOnException<TType, TException>(observable, maxRetryCount, _ => delay, onError, scheduler);
        }





        public static IObservable<TType> RetryOnException<TType, TException>(this IObservable<TType> observable,
            int maxRetryCount, Func<int, TimeSpan> delay, Action<TException, int> onError = default, IScheduler scheduler = default)
            where TException : Exception
        {
            Func<int, IObservable<long>> timerFactory = scheduler is null
                ? attempt => Observable.Timer(delay.Invoke(attempt - 1))
                : attempt => Observable.Timer(delay.Invoke(attempt - 1), scheduler);

            return observable
                .RetryWhen(errors =>
                {
                    var handler = errors
                        .Scan(1, (attempt, ex) =>
                        {
                            if (attempt <= maxRetryCount)
                            {
                                onError?.Invoke(ex as TException, attempt);
                            }
                            else
                            {
                                // Re-throw the original exception, after restoring the state that was saved when the exception was captured.
                                ExceptionDispatchInfo.Capture(ex).Throw();
                            }

                            return attempt + 1;
                        })
                        .Take(maxRetryCount + 1);   // Initial attempt + max retries

                    // Using Concat() to combine all the delay observables into a single sequence.
                    return handler.Select(timerFactory).Concat();
                }
                )
                .Catch<TType, TException>(Observable.Throw<TType>);
        }





        /// <summary>Unlike the Delay operator which time offsets an entire sequence, this method sequentially time offsets
        /// each value emitted by the source sequence. As an example, if 3 values are immediately emitted by <paramref name="observable"/>
        /// and <paramref name="delay"/> is 1 second then the values are re-emitted after one second, two seconds, and three seconds.</summary>
        /// <typeparam name="TType">The type being observed.</typeparam>
        /// <param name="observable">The observable to sequentially time offset each emitted value.</param>
        /// <param name="delay">The interval to time offset an emitted value after processing the previous value.</param>
        /// <returns>An observable that time offsets each value emitted by the source sequence.</returns>
        public static IObservable<TType> EmitAfter<TType>(this IObservable<TType> observable, TimeSpan delay, IScheduler scheduler = default)
        {
            var timer = scheduler is null
                ? Observable.Timer(delay, delay)
                : Observable.Timer(delay, delay, scheduler);

            return observable.Zip(timer, (value, _) => value);
        }
    }
}