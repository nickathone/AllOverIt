using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using System;

namespace AllOverIt.Pipes.Named.Client
{
    public interface IPipeClientConnection<TMessage> : IConnectablePipeConnection<TMessage>
    {
        /// <summary>Event raised when a message is received from the other end of the pipe. Messages are raised
        /// on a background task.</summary>
        public event EventHandler<ConnectionMessageEventArgs<TMessage, IPipeClientConnection<TMessage>>> OnMessageReceived;

        /// <summary>Event raised when the pipe connection is disconnected. This event is raised on a background task.</summary>
        public event EventHandler<ConnectionEventArgs<TMessage, IPipeClientConnection<TMessage>>> OnDisconnected;

        /// <summary>Event raised when an exception is thrown during any read/write operation. This event is raised on a
        /// background task.</summary>
        public event EventHandler<ConnectionExceptionEventArgs<TMessage, IPipeClientConnection<TMessage>>> OnException;

        /// <summary>Gets the connection's server name.</summary>
        public string ServerName { get; }
    }
}