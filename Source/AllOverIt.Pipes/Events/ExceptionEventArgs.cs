using AllOverIt.Assertion;
using System;

namespace AllOverIt.Pipes.Events
{
    public class ExceptionEventArgs : EventArgs
    {
        /// <summary>The exception that was raised.</summary>
        public Exception Exception { get; }

        /// <summary>Constructor.</summary>
        /// <param name="exception">The exception associated with the event.</param>
        public ExceptionEventArgs(Exception exception)
        {
            Exception = exception.WhenNotNull(nameof(exception));
        }
    }
}