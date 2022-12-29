using System;

namespace AllOverIt.Patterns.Pipeline
{
    /// <summary>Represents a synchronous pipeline builder.</summary>
    /// <typeparam name="TIn">The input type for the pipeline.</typeparam>
    /// <typeparam name="TOut">The output type for the pipeline.</typeparam>
    public interface IPipelineBuilder<TIn, TOut>
    {
        /// <summary>Builds the pipeline's steps into a Func that can later be invoked.</summary>
        /// <returns>The pipeline's steps composed to a single Func that can later be invoked.</returns>
        Func<TIn, TOut> Build();
    }
}
