using System;

namespace AllOverIt.Patterns.Pipeline
{
    public interface IPipelineStepBuilder<TIn, TOut>
    {
        Func<TIn, TOut> Build();
    }
}
