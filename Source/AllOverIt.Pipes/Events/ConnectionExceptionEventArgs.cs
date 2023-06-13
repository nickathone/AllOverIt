using AllOverIt.Assertion;
using AllOverIt.Pipes.Connection;
using System;

namespace AllOverIt.Pipes.Events
{
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