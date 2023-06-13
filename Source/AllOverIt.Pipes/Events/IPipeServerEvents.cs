using AllOverIt.Pipes.Connection;
using System;

namespace AllOverIt.Pipes.Events
{
    public interface IPipeServerEvents<TMessage>
    {
        /// <summary>
        /// Invoked whenever a client connects to the server.
        /// </summary>
        event EventHandler<ConnectionEventArgs<TMessage, IPipeServerConnection<TMessage>>> OnClientConnected;

        /// <summary>
        /// Invoked whenever a client disconnects from the server.
        /// </summary>
        event EventHandler<ConnectionEventArgs<TMessage, IPipeServerConnection<TMessage>>> OnClientDisconnected;

        /// <summary>
        /// Invoked whenever a message is received.
        /// </summary>
        event EventHandler<ConnectionMessageEventArgs<TMessage, IPipeServerConnection<TMessage>>> OnMessageReceived;
    }
}