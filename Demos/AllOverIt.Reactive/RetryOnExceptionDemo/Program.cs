using AllOverIt.Reactive.Extensions;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace RetryOnExceptionDemo
{
    internal class Program
    {
        static async Task Main()
        {
            var completion = new TaskCompletionSource<bool>();

            //DemoRetryOnException(TimeSpan.FromSeconds(3), completion);


            DemoRetryOnException(attempt =>
            {
                Console.WriteLine($"...Delay {attempt} seconds - {DateTime.Now}");

                return TimeSpan.FromSeconds(attempt);
            }, completion);


            //DemoEmitAfter(TimeSpan.FromSeconds(1), completion);

            await completion.Task;

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void DemoRetryOnException(TimeSpan retryDelay, TaskCompletionSource<bool> completion)
        {
            Observable
                .Range(1, 5)
                .Select(value =>
                {
                    if (value == 4)
                    {
                        throw new RetryException();
                    }

                    return value;
                })
                .RetryOnException<int, RetryException>(2, retryDelay, (ex, retry) => Console.WriteLine($"Retry {retry} : {ex.Message}"))
                .Subscribe(
                    onNext: value =>
                    {
                        Console.WriteLine($"{value} - {DateTime.Now}");
                    },
                    onError: ex =>
                    {
                        Console.WriteLine();
                        Console.WriteLine("Callstack of expected exception:");
                        Console.WriteLine("--------------------------------");
                        Console.WriteLine(ex.ToString());

                        completion.SetResult(true);
                    });
        }

        private static void DemoRetryOnException(Func<int, TimeSpan> retryDelay, TaskCompletionSource<bool> completion)
        {
            Observable
                .Range(1, 5)
                .Select(value =>
                {
                    if (value == 4)
                    {
                        throw new RetryException();
                    }

                    return value;
                })
                .RetryOnException<int, RetryException>(2, retryDelay, (ex, retry) => Console.WriteLine($"Retry {retry} : {ex.Message}"))
                .Subscribe(
                    onNext: value =>
                    {
                        Console.WriteLine($"{value} - {DateTime.Now}");
                    },
                    onError: ex =>
                    {
                        Console.WriteLine();
                        Console.WriteLine("Callstack of expected exception:");
                        Console.WriteLine("--------------------------------");
                        Console.WriteLine(ex.ToString());

                        completion.SetResult(true);
                    });
        }

        private static void DemoEmitAfter(TimeSpan interval, TaskCompletionSource<bool> completion)
        {
            Observable
                .Range(1, 5)
                .EmitAfter(interval)
                .Subscribe(
                    onNext: value =>
                    {
                        Console.WriteLine($"{value} - {DateTime.Now}");
                    },
                    onCompleted: () =>
                    {
                        Console.WriteLine("Completed");

                        completion.SetResult(true);
                    });
        }
    }
}