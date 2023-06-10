using AllOverIt.Pipes.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Connection
{
    /// <summary>Represents a connection for an underlying <see cref="System.IO.Pipes.PipeStream"/> that can be written to
    /// and receive notification of messages, exceptions, and disconnections.</summary>
    /// <typeparam name="TType">The message type serialized by the connection.</typeparam>
    public interface IPipeConnection<TType>
    {
        /// <summary>Event raised when a message is received from the other end of the pipe. Messages are raised
        /// on a background task.</summary>
        public event EventHandler<ConnectionMessageEventArgs<TType>> OnMessageReceived;

        /// <summary>Event raised when the pipe connection is disconnected. This event is raised on a background task.</summary>
        public event EventHandler<ConnectionEventArgs<TType>> OnDisconnected;

        /// <summary>Event raised when an exception is thrown during any read/write operation. This event is raised on a
        /// background task.</summary>
        public event EventHandler<ConnectionExceptionEventArgs<TType>> OnException;

        /// <summary>Gets the connection's pipe name.</summary>
        public string PipeName { get; }

        /// <summary>Gets the connection's server name. Only applicable for client connections.</summary>
        public string ServerName { get; }

        /// <summary>Indicates if the underlying pipe stream is connected.</summary>
        public bool IsConnected { get; }

        /// <summary>Starts a background task to receive messages from an underlying pipe stream, and allows messages
        /// to be written to the pipe stream.</summary>
        void Connect();

        /// <summary>Terminates the background reader task and disposes of the underlying pipe stream.</summary>
        Task DisconnectAsync();

        /// <summary>Writes the specified <paramref name="value"/> and waits for the other end of the connection
        /// to read it.</summary>
        /// <param name="value">The value to be written to the pipe.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        Task WriteAsync(TType value, CancellationToken cancellationToken = default);

        /// <summary>Gets the username of the connected client.  The client's username will not be available until it has 
        /// written at least once to the pipe (and has set its impersonation level appropriately).</summary>
        /// <returns>The username of the connected client or an <see cref="IOException"/> if the client has not yet
        /// written to the pipe.</returns>
        string GetImpersonationUserName();
    }
}