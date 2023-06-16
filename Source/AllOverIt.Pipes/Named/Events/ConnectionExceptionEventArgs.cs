using AllOverIt.Assertion;
using AllOverIt.Pipes.Named.Client;
using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Server;
using System;

namespace AllOverIt.Pipes.Named.Events
{
    /// <summary>Defines pipe connection event arguments for when an exception is raised.</summary>
    /// <typeparam name="TMessage">The message type serialized on the pipe connection.</typeparam>
    /// <typeparam name="TPipeConnection">The connection type. This will be <see cref="IPipeServerConnection{TMessage}"/> or
    /// <see cref="IPipeClientConnection{TMessage}"/>.</typeparam>
    public sealed class ConnectionExceptionEventArgs<TMessage, TPipeConnection> : ConnectionEventArgs<TMessage, TPipeConnection>
        where TPipeConnection : class, IPipeConnection<TMessage>
    {
        /// <summary>The exception that was raised.</summary>
        public Exception Exception { get; }

        /// <summary>Constructor.</summary>
        /// <param name="connection">The connection associated with the event.</param>
        /// <param name="exception">The exception associated with the event.</param>
        public ConnectionExceptionEventArgs(TPipeConnection connection, Exception exception)
            : base(connection)
        {
            Exception = exception.WhenNotNull(nameof(exception));
        }
    }
}