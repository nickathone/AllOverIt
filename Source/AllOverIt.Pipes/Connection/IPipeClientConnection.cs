using AllOverIt.Pipes.Events;
using System;

namespace AllOverIt.Pipes.Connection
{
    public interface IPipeClientConnection<TMessage> : IPipeConnection<TMessage>
    {
        /// <summary>Event raised when a message is received from the other end of the pipe. Messages are raised
        /// on a background task.</summary>
        public event EventHandler<ConnectionMessageEventArgs<TMessage, IPipeClientConnection<TMessage>>> OnMessageReceived;

        /// <summary>Event raised when the pipe connection is disconnected. This event is raised on a background task.</summary>
        public event EventHandler<ConnectionEventArgs<TMessage, IPipeClientConnection<TMessage>>> OnDisconnected;

        /// <summary>Event raised when an exception is thrown during any read/write operation. This event is raised on a
        /// background task.</summary>
        public event EventHandler<ConnectionExceptionEventArgs<TMessage, IPipeClientConnection<TMessage>>> OnException;

        /// <summary>Gets the connection's server name. Only applicable for client connections.</summary>
        public string ServerName { get; }
    }
}