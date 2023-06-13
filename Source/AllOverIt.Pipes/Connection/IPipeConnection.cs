using AllOverIt.Pipes.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Connection
{
    /// <summary>Represents a connection for an underlying <see cref="System.IO.Pipes.PipeStream"/> that can be written to
    /// and receive notification of messages, exceptions, and disconnections.</summary>
    /// <typeparam name="TMessage">The message type serialized by the connection.</typeparam>
    public interface IPipeConnection<TMessage> : IAsyncDisposable
    {
        /// <summary>Gets the connection's pipe name.</summary>
        public string PipeName { get; }

        /// <summary>Indicates if the underlying pipe stream is connected.</summary>
        public bool IsConnected { get; }

        /// <summary>Starts a background task to receive messages from an underlying pipe stream, and allows messages
        /// to be written to the pipe stream.</summary>
        void Connect();

        /// <summary>Terminates the background reader task and disposes of the underlying pipe stream.</summary>
        Task DisconnectAsync();

        /// <summary>Serializes the specified <paramref name="value"/> and waits for the other end of the connection
        /// to read it.</summary>
        /// <param name="value">The value to be serialized to the pipe.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task WriteAsync(TMessage value, CancellationToken cancellationToken = default);
    }
}