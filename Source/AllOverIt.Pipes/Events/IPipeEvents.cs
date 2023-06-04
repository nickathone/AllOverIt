using System;

namespace AllOverIt.Pipes.Events
{
    public interface IPipeEvents<TType>
    {
        /// <summary>
        /// Invoked whenever a message is received.
        /// </summary>
        event EventHandler<ConnectionMessageEventArgs<TType>> OnMessageReceived;

        /// <summary>
        /// Invoked whenever an exception is thrown during a read or write operation on the named pipe.
        /// </summary>
        event EventHandler<ExceptionEventArgs> OnException;
    }
}