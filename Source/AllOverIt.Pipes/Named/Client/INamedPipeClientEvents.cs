using AllOverIt.Pipes.Named.Events;
using System;

namespace AllOverIt.Pipes.Named.Client
{
    public interface INamedPipeClientEvents<TMessage>
    {
        /// <summary>
        /// Invoked after each the client connect to the server (include reconnects).
        /// </summary>
        event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnConnected;

        /// <summary>
        /// Invoked when the client disconnects from the server (e.g., the pipe is closed or broken).
        /// </summary>
        event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnDisconnected;

        /// <summary>
        /// Invoked whenever a message is received.
        /// </summary>
        event EventHandler<NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnMessageReceived;
    }
}