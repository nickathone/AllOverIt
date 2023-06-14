using AllOverIt.Assertion;
using AllOverIt.Pipes.Connection;
using System;

namespace AllOverIt.Pipes.Events
{
    public class ConnectionEventArgs<TMessage, TPipeConnection> : EventArgs
        where TPipeConnection : class, IPipeConnection<TMessage>
    {
        /// <summary>The connection associated with the event.</summary>
        public TPipeConnection Connection { get; }              // TODO: Look at reducing visibility of Connect/Disconnect/Write available in IPipeConnection<TMessage>
                                                                //       PipeServer needs to be able to Disconnect / Write

        /// <summary>Constructor.</summary>
        /// <param name="connection">The connection associated with the event.</param>
        public ConnectionEventArgs(TPipeConnection connection)
        {
            Connection = connection.WhenNotNull(nameof(connection));
        }
    }
}