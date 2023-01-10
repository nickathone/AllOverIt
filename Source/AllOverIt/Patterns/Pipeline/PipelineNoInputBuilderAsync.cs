using AllOverIt.Assertion;
using System;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.Pipeline
{
    // Begins a new asynchronous pipline that takes no initial input
    internal sealed class PipelineNoInputBuilderAsync<TOut> : IPipelineBuilderAsync<TOut>
    {
        private readonly Func<Task<TOut>> _step;

        public PipelineNoInputBuilderAsync(Func<Task<TOut>> step)
        {
            _step = step.WhenNotNull(nameof(step));
        }

        public Func<Task<TOut>> Build()
        {
            return _step;
        }
    }

    // Appends to an asynchronous pipline that takes no initial input
    internal sealed class PipelineNoInputBuilderAsync<TPrevOut, TNextOut> : IPipelineBuilderAsync<TNextOut>
    {
        private readonly IPipelineBuilderAsync<TPrevOut> _prevStep;
        private readonly Func<TPrevOut, Task<TNextOut>> _step;

        public PipelineNoInputBuilderAsync(IPipelineBuilderAsync<TPrevOut> prevStep, Func<TPrevOut, Task<TNextOut>> step)
        {
            _prevStep = prevStep.WhenNotNull(nameof(prevStep));
            _step = step.WhenNotNull(nameof(step));
        }

        // Create a func that invokes the previous func and uses its result as the input to the next func (step)
        public Func<Task<TNextOut>> Build()
        {
            async Task<TNextOut> func()
            {
                var prevOutput = await _prevStep
                    .Build()
                    .Invoke()
                    .ConfigureAwait(false);

                return await _step
                    .Invoke(prevOutput)
                    .ConfigureAwait(false);
            }

            return func;
        }
    }
}
