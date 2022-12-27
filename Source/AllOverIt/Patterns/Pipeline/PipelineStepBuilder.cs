using AllOverIt.Assertion;
using System;

namespace AllOverIt.Patterns.Pipeline
{
    public class PipelineStepBuilder<TIn, TOut> : IPipelineStepBuilder<TIn, TOut>
    {
        private readonly Func<TIn, TOut> _step;

        public PipelineStepBuilder(Func<TIn, TOut> step)
        {
            _step = step.WhenNotNull(nameof(step));
        }

        public Func<TIn, TOut> Build()
        {
            return _step;
        }
    }

    public class PipelineStepBuilder<TIn, TPrevOut, TNextOut> : IPipelineStepBuilder<TIn, TNextOut>
    {
        private readonly IPipelineStepBuilder<TIn, TPrevOut> _prevStep;
        private readonly Func<TPrevOut, TNextOut> _step;

        public PipelineStepBuilder(IPipelineStepBuilder<TIn, TPrevOut> prevStep, Func<TPrevOut, TNextOut> step)
        {
            _prevStep = prevStep.WhenNotNull(nameof(prevStep));
            _step = step.WhenNotNull(nameof(step));
        }

        public Func<TIn, TNextOut> Build()
        {
            TNextOut func(TIn input)
            {
                var prevOutput = _prevStep.Build().Invoke(input);
                return _step.Invoke(prevOutput);

            }

            return func;
        }
    }
}
