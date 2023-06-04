using System;

namespace AllOverIt.Pipes.Events
{
    public interface IPipeServerEvents<TType>
    {
        /// <summary>
        /// Invoked whenever a client connects to the server.
        /// </summary>
        event EventHandler<ConnectionEventArgs<TType>> OnClientConnected;

        /// <summary>
        /// Invoked whenever a client disconnects from the server.
        /// </summary>
        event EventHandler<ConnectionEventArgs<TType>> OnClientDisconnected;
    }
}