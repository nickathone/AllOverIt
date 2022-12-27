using AllOverIt.Patterns.Pipeline;
using AllOverIt.Patterns.Pipeline.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PipelineAsyncDemo.Steps;
using System;
using System.Threading.Tasks;

namespace PipelineAsyncDemo
{
    public delegate Task<string> IntegerPipelineProcessorAsync(int value);

    internal class Program
    {
        static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddSingleton<AddFractionStep>();

            // Showing how to register using a IntegerPipelineProcessorAsync delegate
            services.AddSingleton(provider =>
            {
                var addFractionStep = provider.GetRequiredService<AddFractionStep>();

                var pipeline = PipelineBuilder

                   // Using a class type - implemeted as async
                   // Only <TPipelineStep, TIn, TOut> because there is no previous input
                   .PipeAsync<DoubleStepAsync, int, int>()

                   // Using a class instance - implemeted as async
                   .PipeAsync(new DoubleStepAsync())

                   // Using a class type (TPipelineStep) - implemeted as non-async
                   // TPipelineStep, TIn, TPrevOut, TNextOut
                   //   TIn is the original input type
                   //   TPrevOut is the result type from the previous call
                   //   TNextOut is the result type for this next step
                   .Pipe<DoubleStep, int, int, int>()

                   // Using a non-async Func
                   .Pipe(value => value * 2)

                   .PipeAsync(addFractionStep)           // << Much easier this way if using DI

                   .Pipe(value => $"{value}")

                   // Get the resulting Func that has everything merged together
                   .Build();

                return new IntegerPipelineProcessorAsync(pipeline);
            });

            var serviceProvider = services.BuildServiceProvider();

            var pipeline = serviceProvider.GetRequiredService<IntegerPipelineProcessorAsync>();

            var result = await pipeline.Invoke(5);

            Console.WriteLine($"{result}");

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}