using System;

namespace AllOverIt.Pipes.Exceptions
{
    public class PipeConnectionException : Exception
    {
        public PipeConnectionException()
        {
        }

        public PipeConnectionException(string message)
            : base(message)
        {
        }
    }
}