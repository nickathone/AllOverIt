using System;

namespace AllOverIt.Pipes.Named.Events
{
    public interface IPipeEvents<TMessage>
    {
        /// <summary>
        /// Invoked whenever an exception is thrown during a read or write operation on the named pipe.
        /// </summary>
        event EventHandler<ExceptionEventArgs> OnException;
    }
}