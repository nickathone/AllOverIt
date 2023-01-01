using AllOverIt.Async;
using AllOverIt.Extensions;
using AllOverIt.Patterns.Pipeline;
using AllOverIt.Patterns.Pipeline.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineParallelDemo
{
    internal class Program
    {
        static async Task Main()
        {
            // These represent what could be more complex pipelines (with multiple steps), running in background threads or tasks
            var minValuePipeline = PipelineBuilder.PipeAsync<IEnumerable<double>, double>(numbers => Task.FromResult(numbers.Min())).Build();
            var maxValuePipeline = PipelineBuilder.PipeAsync<IEnumerable<double>, double>(numbers => Task.FromResult(numbers.Max())).Build();
            var avgValuePipeline = PipelineBuilder.PipeAsync<IEnumerable<double>, double>(numbers => Task.FromResult(numbers.Average())).Build();

            // calculateStats could have been created as a pipeline as shown in the commented block
            /*
                var statsPipeline = PipelineBuilder.PipeAsync<IEnumerable<double>, Stats>(async data =>
                {
                    var numbers = data.AsReadOnlyCollection();

                    var (min, max, avg) = await TaskHelper
                        .WhenAll(
                            minValuePipeline.Invoke(numbers),
                            maxValuePipeline.Invoke(numbers),
                            avgValuePipeline.Invoke(numbers)
                        ).ConfigureAwait(false);

                    return new Stats(min, max, avg);
                });
            */

#pragma warning disable IDE0039 // Use local function
            Func<IEnumerable<double>, Task<Stats>> calculateStats = async data =>
            {
                var numbers = data.AsReadOnlyCollection();

                var (min, max, avg) = await TaskHelper
                    .WhenAll(
                        minValuePipeline.Invoke(numbers),
                        maxValuePipeline.Invoke(numbers),
                        avgValuePipeline.Invoke(numbers)
                    ).ConfigureAwait(false);

                return new Stats(min, max, avg);
            };
#pragma warning restore IDE0039 // Use local function

            var pipeline = PipelineBuilder

                // Scale all numbers between 0 and 1 - assuming all values > 0
                .PipeAsync<IEnumerable<double>, IReadOnlyCollection<double>>(data =>
                {
                    var numbers = data.AsReadOnlyCollection();

                    var max = numbers.Max();

                    var newValues = numbers.SelectAsReadOnlyCollection(number => number / max);

                    return Task.FromResult(newValues);
                })

                // Takes the output from the previous step and runs 3 calcs in parallel before returning a Stats instance
                .PipeAsync(calculateStats)
                
                .Build();

            var input = Enumerable
                .Range(1, 100)
                .Select(value => (double) value);

            var result = await pipeline.Invoke(input);

            Console.WriteLine(result.ToString());

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}