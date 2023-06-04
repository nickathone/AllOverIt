using System;

namespace AllOverIt.Pipes.Events
{
    public interface IPipeClientEvents<TType>
    {
        /// <summary>
        /// Invoked after each the client connect to the server (include reconnects).
        /// </summary>
        event EventHandler<ConnectionEventArgs<TType>> OnConnected;

        /// <summary>
        /// Invoked when the client disconnects from the server (e.g., the pipe is closed or broken).
        /// </summary>
        event EventHandler<ConnectionEventArgs<TType>> OnDisconnected;
    }
}