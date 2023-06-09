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

        /// <summary>Establishes a new connection and a background task to receive all new messages.</summary>
        void Connect();

        /// <summary></summary>
        Task DisconnectAsync();

        /// <summary></summary>
        Task WriteAsync(TType value, CancellationToken cancellationToken = default);

        // Gets the username of the connected client.  Note that we will not have access to the client's
        // username until it has written at least once to the pipe (and has set its impersonationLevel
        // argument appropriately).
        string GetImpersonationUserName();
    }
}