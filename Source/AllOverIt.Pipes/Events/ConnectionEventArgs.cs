using AllOverIt.Assertion;
using AllOverIt.Pipes.Connection;
using AllOverIt.Pipes.Server;
using System;

namespace AllOverIt.Pipes.Events
{
    /// <summary>Defines pipe connection related event arguments.</summary>
    /// <typeparam name="TMessage">The message type serialized on the pipe connection.</typeparam>
    /// <typeparam name="TPipeConnection">The connection type. This will be <see cref="IPipeServerConnection{TMessage}"/> or
    /// <see cref="IPipeServerConnection{TMessage}"/>.</typeparam>
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