using System;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.Pipeline.Extensions
{
    public static class PipelineStepBuilderExtensions
    {
        /// <summary>Adds a new pipeline step, implemented as a <see cref="Func{TPrevOut, TNextOut}"/>, to an existing pipeline step builder.</summary>
        /// <typeparam name="TIn">The input type for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline step builder representing the step just added to the pipeline.</returns>
        public static IPipelineStepBuilder<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(
            this IPipelineStepBuilder<TIn, TPrevOut> prevStep, Func<TPrevOut, TNextOut> step)
        {
            return new PipelineStepBuilder<TIn, TPrevOut, TNextOut>(prevStep, step);
        }

        /// <summary>Adds a new pipeline step, implemented as a <see cref="IPipelineStep{TPrevOut, TNextOut}"/>, to an existing pipeline step builder.</summary>
        /// <typeparam name="TIn">The input type for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <param name="step">The step to be added to the pipeline builder.</param>
        /// <returns>A new pipeline step builder representing the step just added to the pipeline.</returns>
        public static IPipelineStepBuilder<TIn, TNextOut> Pipe<TIn, TPrevOut, TNextOut>(
            this IPipelineStepBuilder<TIn, TPrevOut> prevStep, IPipelineStep<TPrevOut, TNextOut> step)
        {
            return Pipe(prevStep, step.AsFunc());
        }

        /// <summary>Adds a new pipeline step, implemented as a <see cref="IPipelineStep{TPrevOut, TNextOut}"/>, to an existing pipeline step builder.</summary>
        /// <typeparam name="TPipelineStep">The pipeline step type, implemented as a <see cref="IPipelineStep{TPrevOut, TNextOut}"/>.</typeparam>
        /// <typeparam name="TIn">The input type for the first step in the pipeline.</typeparam>
        /// <typeparam name="TPrevOut">The output type from the previous step in the pipeline.</typeparam>
        /// <typeparam name="TNextOut">The output type for the step being added to the pipeline.</typeparam>
        /// <param name="prevStep">The builder instance representing the previous step in the pipeline.</param>
        /// <returns>A new pipeline step builder representing the step just added to the pipeline.</returns>
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
