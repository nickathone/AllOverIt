using System;

namespace AllOverIt.Patterns.Pipeline.Extensions
{
    public static class PipelineStepBuilderExtensions
    {
        public static IPipelineStepBuilder<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(
            this IPipelineStepBuilder<TIn, TPrevOut> prevStep,
            Func<TPrevOut, TNextOut> nextStep)
        {
            return new PipelineStepBuilder<TIn, TPrevOut, TNextOut>(prevStep, nextStep);
        }
    }
}
