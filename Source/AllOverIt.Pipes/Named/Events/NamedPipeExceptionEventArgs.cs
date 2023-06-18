using AllOverIt.Assertion;
using System;

namespace AllOverIt.Pipes.Named.Events
{
    public class NamedPipeExceptionEventArgs : EventArgs
    {
        /// <summary>The exception that was raised.</summary>
        public Exception Exception { get; }

        /// <summary>Constructor.</summary>
        /// <param name="exception">The exception associated with the event.</param>
        public NamedPipeExceptionEventArgs(Exception exception)
        {
            Exception = exception.WhenNotNull(nameof(exception));
        }
    }
}