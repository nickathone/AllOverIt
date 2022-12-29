using AllOverIt.Assertion;
using System;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.Pipeline
{
    internal sealed class PipelineBuilderAsync<TIn, TOut> : IPipelineBuilderAsync<TIn, TOut>
    {
        private readonly Func<TIn, Task<TOut>> _step;

        public PipelineBuilderAsync(Func<TIn, Task<TOut>> step)
        {
            _step = step.WhenNotNull(nameof(step));
        }

        public Func<TIn, Task<TOut>> Build()
        {
            return _step;
        }
    }

    internal sealed class PipelineBuilderAsync<TIn, TPrevOut, TNextOut> : IPipelineBuilderAsync<TIn, TNextOut>
    {
        private readonly IPipelineBuilderAsync<TIn, TPrevOut> _prevStep;
        private readonly Func<TPrevOut, Task<TNextOut>> _step;

        public PipelineBuilderAsync(IPipelineBuilderAsync<TIn, TPrevOut> prevStep, Func<TPrevOut, Task<TNextOut>> step)
        {
            _prevStep = prevStep.WhenNotNull(nameof(prevStep));
            _step = step.WhenNotNull(nameof(step));
        }

        // Create a func that invokes the previous func and uses its result as the input to the next func (step)
        public Func<TIn, Task<TNextOut>> Build()
        {
            async Task<TNextOut> func(TIn input)
            {
                var prevOutput = await _prevStep
                    .Build()
                    .Invoke(input)
                    .ConfigureAwait(false);

                return await _step
                    .Invoke(prevOutput)
                    .ConfigureAwait(false);
            }

            return func;
        }
    }
}
