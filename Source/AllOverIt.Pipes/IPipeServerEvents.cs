using System;

namespace AllOverIt.Pipes
{
    public interface IPipeServerEvents<TType>
    {
        /// <summary>
        /// Invoked whenever a client connects to the server.
        /// </summary>
        event EventHandler<ConnectionEventArgs<TType>> ClientConnected;

        /// <summary>
        /// Invoked whenever a client disconnects from the server.
        /// </summary>
        event EventHandler<ConnectionEventArgs<TType>> ClientDisconnected;
    }
}