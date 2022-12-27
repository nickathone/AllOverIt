using AllOverIt.Assertion;
using System;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.Pipeline.Extensions
{
    public static class PipelineStepExtensions
    {
        public static Func<TIn, TOut> AsFunc<TIn, TOut>(this IPipelineStep<TIn, TOut> step)
        {
            _ = step.WhenNotNull(nameof(step));

            return step.Execute;
        }

        public static Func<TIn, Task<TOut>> AsFunc<TIn, TOut>(this IPipelineStepAsync<TIn, TOut> step)
        {
            _ = step.WhenNotNull(nameof(step));

            return step.ExecuteAsync;
        }
    }        
}
