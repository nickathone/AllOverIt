using System;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.Pipeline
{
    public interface IPipelineBuilderAsync<TIn, TOut>
    {
        Func<TIn, Task<TOut>> Build();
    }
}
