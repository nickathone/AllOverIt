﻿using System;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.Pipeline
{
    /// <summary>Represents an asynchronous pipeline builder.</summary>
    /// <typeparam name="TIn">The input type for the pipeline.</typeparam>
    /// <typeparam name="TOut">The output type for the pipeline.</typeparam>
    public interface IPipelineBuilderAsync<TIn, TOut>
    {
        /// <summary>Builds the pipeline's steps into a Func that can later be invoked.</summary>
        /// <returns>The pipeline's steps composed to a single Func that can later be invoked.</returns>
        Func<TIn, Task<TOut>> Build();
    }
}
