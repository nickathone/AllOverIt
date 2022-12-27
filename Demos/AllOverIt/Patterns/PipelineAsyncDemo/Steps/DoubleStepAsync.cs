using AllOverIt.Patterns.Pipeline;
using System.Threading.Tasks;

namespace PipelineAsyncDemo.Steps
{
    internal sealed class DoubleStepAsync : IPipelineStepAsync<int, int>
    {
        public Task<int> ExecuteAsync(int input)
        {
            return Task.FromResult(input * 2);
        }
    }
}