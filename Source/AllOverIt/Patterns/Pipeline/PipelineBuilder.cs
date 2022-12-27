using AllOverIt.Patterns.Pipeline.Extensions;
using System;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.Pipeline
{
    public static class PipelineBuilder
    {
        public static IPipelineStepBuilder<TIn, TOut> Pipe<TIn, TOut>(Func<TIn, TOut> step)
        {
            return new PipelineStepBuilder<TIn, TOut>(step);
        }

        public static IPipelineStepBuilder<TIn, TOut> Pipe<TIn, TOut>(IPipelineStep<TIn, TOut> step)
        {
            // AsFunc() performs a null check
            return new PipelineStepBuilder<TIn, TOut>(step.AsFunc());
        }

        public static IPipelineStepBuilder<TIn, TOut> Pipe<TPipelineStep, TIn, TOut>() where TPipelineStep : IPipelineStep<TIn, TOut>, new()
        {
            var step = new TPipelineStep();
            return Pipe(step);
        }

        public static IPipelineStepBuilderAsync<TIn, TOut> PipeAsync<TIn, TOut>(Func<TIn, Task<TOut>> step)
        {
            return new PipelineStepBuilderAsync<TIn, TOut>(step);
        }

        public static IPipelineStepBuilderAsync<TIn, TOut> PipeAsync<TIn, TOut>(IPipelineStepAsync<TIn, TOut> step)
        {
            // AsFunc() performs a null check
            return new PipelineStepBuilderAsync<TIn, TOut>(step.AsFunc());
        }

        public static IPipelineStepBuilderAsync<TIn, TOut> PipeAsync<TPipelineStep, TIn, TOut>() where TPipelineStep : IPipelineStepAsync<TIn, TOut>, new()
        {
            var step = new TPipelineStep();

            return PipeAsync(step);
        }
    }
}
