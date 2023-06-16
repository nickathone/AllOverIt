using System;

namespace AllOverIt.Pipes.Named.Exceptions
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