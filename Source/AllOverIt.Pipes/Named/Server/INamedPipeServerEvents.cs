using AllOverIt.Pipes.Named.Events;
using System;

namespace AllOverIt.Pipes.Named.Server
{
    public interface INamedPipeServerEvents<TMessage>
    {
        /// <summary>
        /// Invoked whenever a client connects to the server.
        /// </summary>
        event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>> OnClientConnected;

        /// <summary>
        /// Invoked whenever a client disconnects from the server.
        /// </summary>
        event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeServerConnection<TMessage>>> OnClientDisconnected;

        /// <summary>
        /// Invoked whenever a message is received.
        /// </summary>
        event EventHandler<NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeServerConnection<TMessage>>> OnMessageReceived;
    }
}