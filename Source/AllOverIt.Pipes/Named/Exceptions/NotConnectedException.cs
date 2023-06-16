using System;

namespace AllOverIt.Pipes.Named.Exceptions
{
    public class NotConnectedException : Exception
    {
        public NotConnectedException()
        {
        }

        public NotConnectedException(string message)
            : base(message)
        {
        }
    }
}