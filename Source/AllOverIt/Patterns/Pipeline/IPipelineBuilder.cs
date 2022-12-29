using System;

namespace AllOverIt.Patterns.Pipeline
{
    public interface IPipelineBuilder<TIn, TOut>
    {
        Func<TIn, TOut> Build();
    }
}
