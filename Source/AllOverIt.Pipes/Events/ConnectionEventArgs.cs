using AllOverIt.Assertion;
using AllOverIt.Pipes.Connection;
using System;

namespace AllOverIt.Pipes.Events
{
    public class ConnectionEventArgs<TType> : EventArgs
    {
        /// <summary>The connection associated with the event.</summary>
        public IPipeConnection<TType> Connection { get; }

        /// <summary>Constructor.</summary>
        /// <param name="connection">The connection associated with the event.</param>
        public ConnectionEventArgs(IPipeConnection<TType> connection)
        {
            Connection = connection.WhenNotNull(nameof(connection));
        }
    }
}