using AllOverIt.Assertion;
using AllOverIt.Pipes.Connection;
using System;

namespace AllOverIt.Pipes.Events
{
    public class ConnectionEventArgs<TMessage, TPipeConnection> : EventArgs
        where TPipeConnection : class, IPipeConnection<TMessage>
    {
        /// <summary>The connection associated with the event.</summary>
        public TPipeConnection Connection { get; }

        /// <summary>Constructor.</summary>
        /// <param name="connection">The connection associated with the event.</param>
        public ConnectionEventArgs(TPipeConnection connection)
        {
            Connection = connection.WhenNotNull(nameof(connection));
        }
    }
}