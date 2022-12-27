using AllOverIt.Assertion;
using System;

namespace AllOverIt.Patterns.Pipeline.Extensions
{
    public static class PipelineStepBuilderExtensions
    {
        public static IPipelineStepBuilder<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(
            this IPipelineStepBuilder<TIn, TPrevOut> prevStep,
            Func<TPrevOut, TNextOut> step)
        {
            return new PipelineStepBuilder<TIn, TPrevOut, TNextOut>(prevStep, step);
        }

        public static IPipelineStepBuilder<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(
            this IPipelineStepBuilder<TIn, TPrevOut> prevStep,
            IPipelineStep<TPrevOut, TNextOut> step)
        {
            return new PipelineStepBuilder<TIn, TPrevOut, TNextOut>(prevStep, step);
        }

        public static IPipelineStepBuilder<TIn, TNextOut> Pipe<TPipelineStep, TIn, TPrevOut, TNextOut>(
            this IPipelineStepBuilder<TIn, TPrevOut> prevStep) where TPipelineStep : IPipelineStep<TPrevOut, TNextOut>, new()
        {
            var step = new TPipelineStep();

            return new PipelineStepBuilder<TIn, TPrevOut, TNextOut>(prevStep, step);
        }
    }        
}
