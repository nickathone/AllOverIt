using AllOverIt.Assertion;
using System;

namespace AllOverIt.Patterns.Pipeline.Extensions
{
    public static class PipelineStepExtensions
    {
        public static Func<TIn, TOut> AsFunc<TIn, TOut>(this IPipelineStep<TIn, TOut> step)
        {
            _ = step.WhenNotNull(nameof(step));

            return step.Execute;
        }
    }        
}
