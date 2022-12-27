using System;

namespace AllOverIt.Patterns.Pipeline
{
    public static class PipelineBuilder
    {
        public static IPipelineStepBuilder<TIn, TOut> Pipe<TIn, TOut>(Func<TIn, TOut> step)
        {
            return new PipelineStepBuilder<TIn, TOut>(step);
        }
    }
}
