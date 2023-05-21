using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes
{
    public interface IPipeEvents<TType>
    {
        /// <summary>
        /// Invoked whenever a message is received.
        /// </summary>
        event EventHandler<ConnectionMessageEventArgs<TType>> OnMessageReceived;

        /// <summary>
        /// Invoked whenever an exception is thrown during a read or write operation on the named pipe.
        /// </summary>
        event EventHandler<ExceptionEventArgs> OnException;
    }

    public interface IPipe<TType> : IAsyncDisposable
    {
        /// <summary>Asynchronously sends a message to all connected clients.</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        Task WriteAsync(TType message, CancellationToken cancellationToken = default);
    }
}