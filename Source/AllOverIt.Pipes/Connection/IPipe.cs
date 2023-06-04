using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Connection
{
    public interface IPipe<TType> : IAsyncDisposable
    {
        /// <summary>Asynchronously sends a message to all connected clients.</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        Task WriteAsync(TType message, CancellationToken cancellationToken = default);
    }
}