using System;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.Pipeline
{
    public interface IPipelineStepBuilderAsync<TIn, TOut>
    {
        Func<TIn, Task<TOut>> Build();
    }
}
