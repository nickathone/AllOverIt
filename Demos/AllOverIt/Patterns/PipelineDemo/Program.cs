using AllOverIt.Patterns.Pipeline;
using AllOverIt.Patterns.Pipeline.Extensions;
using PipelineDemo.Steps;
using System;

namespace PipelineDemo
{
    internal class Program
    {
        static void Main()
        {
            var pipeline = PipelineBuilder

               // Using a class type
               // Only <TPipelineStep, TIn, TOut> because there is no previous input
               .Pipe<DoubleStep, int, int>()

               // Using a class instance
               .Pipe(new DoubleStep())

               // TIn is the original input type
               // TPrevOut is the result type from the previous call
               // TNextOut is the result type for this next step
               //
               // TPipelineStep, TIn, TPrevOut, TNextOut
               .Pipe<DoubleStep, int, int, int>()
               
               // Using a Func
               .Pipe(value => value * 2)

               .Pipe(new AddFractionStep())

               .Pipe(value => $"{value}")

               // Build the resulting Func that merges it all together
               .Build();

            var result = pipeline.Invoke(15);

            Console.WriteLine($"{result}");

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}