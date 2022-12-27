using System;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.Pipeline.Extensions
{
    public static class PipelineStepBuilderExtensions
    {
        // ====== sync
        public static IPipelineStepBuilder<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(
            this IPipelineStepBuilder<TIn, TPrevOut> prevStep, Func<TPrevOut, TNextOut> step)
        {
            return new PipelineStepBuilder<TIn, TPrevOut, TNextOut>(prevStep, step);
        }

        public static IPipelineStepBuilder<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(
            this IPipelineStepBuilder<TIn, TPrevOut> prevStep, IPipelineStep<TPrevOut, TNextOut> step)
        {
            return Pipe(prevStep, step.AsFunc());
        }

        public static IPipelineStepBuilder<TIn, TNextOut> Pipe<TPipelineStep, TIn, TPrevOut, TNextOut>(
            this IPipelineStepBuilder<TIn, TPrevOut> prevStep) where TPipelineStep : IPipelineStep<TPrevOut, TNextOut>, new()
        {
            var step = new TPipelineStep();

            return Pipe(prevStep, step);
        }

        // ====== async

        public static IPipelineStepBuilderAsync<TIn, TNextOut> PipeAsync<TIn, TPrevOut, TNextOut>(
           this IPipelineStepBuilderAsync<TIn, TPrevOut> prevStep, Func<TPrevOut, Task<TNextOut>> step)
        {
            return new PipelineStepBuilderAsync<TIn, TPrevOut, TNextOut>(prevStep, step);
        }

        public static IPipelineStepBuilderAsync<TIn, TNextOut> PipeAsync<TIn, TPrevOut, TNextOut>(
            this IPipelineStepBuilderAsync<TIn, TPrevOut> prevStep, IPipelineStepAsync<TPrevOut, TNextOut> step)
        {
            return PipeAsync(prevStep, step.AsFunc());
        }

        public static IPipelineStepBuilderAsync<TIn, TNextOut> PipeAsync<TPipelineStep, TIn, TPrevOut, TNextOut>(
            this IPipelineStepBuilderAsync<TIn, TPrevOut> prevStep) where TPipelineStep : IPipelineStepAsync<TPrevOut, TNextOut>, new()
        {
            var step = new TPipelineStep();

            return PipeAsync(prevStep, step);
        }

        // ====== allows pipe (non-async) back to async

        public static IPipelineStepBuilderAsync<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(
           this IPipelineStepBuilderAsync<TIn, TPrevOut> prevStep, Func<TPrevOut, TNextOut> step)
        {
            Task<TNextOut> stepAsync(TPrevOut result) => Task.FromResult(step.Invoke(result));

            return new PipelineStepBuilderAsync<TIn, TPrevOut, TNextOut>(prevStep, stepAsync);
        }

        public static IPipelineStepBuilderAsync<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(
            this IPipelineStepBuilderAsync<TIn, TPrevOut> prevStep, IPipelineStep<TPrevOut, TNextOut> step)
        {
            return Pipe(prevStep, step.AsFunc());
        }

        public static IPipelineStepBuilderAsync<TIn, TNextOut> Pipe<TPipelineStep, TIn, TPrevOut, TNextOut>(
            this IPipelineStepBuilderAsync<TIn, TPrevOut> prevStep) where TPipelineStep : IPipelineStep<TPrevOut, TNextOut>, new()
        {
            var step = new TPipelineStep();

            return Pipe(prevStep, step);
        }
    }        
}
