using System;

namespace AllOverIt.Pipes.Exceptions
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