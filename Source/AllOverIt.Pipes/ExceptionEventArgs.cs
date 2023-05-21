using AllOverIt.Assertion;
using System;

namespace AllOverIt.Pipes
{
    public class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public ExceptionEventArgs(Exception exception)
        {
            Exception = exception.WhenNotNull(nameof(exception));
        }
    }
}