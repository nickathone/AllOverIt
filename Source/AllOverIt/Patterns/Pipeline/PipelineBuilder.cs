using System;

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
            return new PipelineStepBuilder<TIn, TOut>(step);
        }

        public static IPipelineStepBuilder<TIn, TOut> Pipe<TPipelineStep, TIn, TOut>() where TPipelineStep : IPipelineStep<TIn, TOut>, new()
        {
            var step = new TPipelineStep();
            return new PipelineStepBuilder<TIn, TOut>(step);
        }
    }
}
