using AllOverIt.Assertion;
using System;

namespace AllOverIt.Pipes
{
    public class ConnectionExceptionEventArgs<T> : ConnectionEventArgs<T>
    {
        /// <summary>
        /// The exception that was thrown
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="exception"></param>
        public ConnectionExceptionEventArgs(PipeConnection<T> connection, Exception exception)
            : base(connection)
        {
            Exception = exception.WhenNotNull(nameof(exception));
        }
    }


}