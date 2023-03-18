using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Process
{
    public interface IProcessExecutor : IDisposable
    {
        Task<ProcessExecutorResult> ExecuteAsync(CancellationToken cancellationToken = default);
        Task<ProcessExecutorBufferedResult> ExecuteBufferedAsync(CancellationToken cancellationToken = default);
    }
}
