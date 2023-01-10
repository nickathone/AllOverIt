using AllOverIt.Assertion;
using AllOverIt.Patterns.Pipeline;
using ReactiveUI;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AllOverIt.ReactiveUI.CommandPipeline
{
    // Adapts a ReactiveCommand<TIn, TOut> as an IPipelineStepAsync<TIn, TOut>
    internal class ReactiveCommandPipelineStep<TIn, TOut> : IPipelineStepAsync<TIn, TOut>
    {
        private readonly ReactiveCommand<TIn, TOut> _command;

        public ReactiveCommandPipelineStep(ReactiveCommand<TIn, TOut> command)
        {
            _command = command.WhenNotNull(nameof(command));
        }

        public async Task<TOut> ExecuteAsync(TIn input)
        {
            return await _command.Execute(input);
        }
    }
}