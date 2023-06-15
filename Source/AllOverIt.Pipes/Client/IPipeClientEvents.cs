using AllOverIt.Pipes.Connection;
using AllOverIt.Pipes.Events;
using System;

namespace AllOverIt.Pipes.Client
{
    public interface IPipeClientEvents<TMessage>
    {
        /// <summary>
        /// Invoked after each the client connect to the server (include reconnects).
        /// </summary>
        event EventHandler<ConnectionEventArgs<TMessage, IPipeClientConnection<TMessage>>> OnConnected;

        /// <summary>
        /// Invoked when the client disconnects from the server (e.g., the pipe is closed or broken).
        /// </summary>
        event EventHandler<ConnectionEventArgs<TMessage, IPipeClientConnection<TMessage>>> OnDisconnected;

        /// <summary>
        /// Invoked whenever a message is received.
        /// </summary>
        event EventHandler<ConnectionMessageEventArgs<TMessage, IPipeClientConnection<TMessage>>> OnMessageReceived;
    }
}