using System.Threading.Tasks;

namespace AllOverIt.Patterns.Pipeline
{
    public interface IPipelineStep<TIn, TOut>
    {
        TOut Execute(TIn input);
    }

    public interface IPipelineStepAsync<TIn, TOut>
    {
        Task<TOut> ExecuteAsync(TIn input);
    }
}
