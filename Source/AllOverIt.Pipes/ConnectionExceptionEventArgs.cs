using AllOverIt.Assertion;
using System;

namespace AllOverIt.Pipes
{
    public sealed class ConnectionExceptionEventArgs<TType> : ConnectionEventArgs<TType>
    {
        /// <summary>The exception that was raised.</summary>
        public Exception Exception { get; }

        /// <summary>Constructor.</summary>
        /// <param name="connection">The connection associated with the event.</param>
        /// <param name="exception">The exception associated with the event.</param>
        public ConnectionExceptionEventArgs(IPipeConnection<TType> connection, Exception exception)
            : base(connection)
        {
            Exception = exception.WhenNotNull(nameof(exception));
        }
    }
}