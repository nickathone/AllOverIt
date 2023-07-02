using System;

namespace AllOverIt.Pipes.Named.Events
{
    /// <summary>Declares named pipe event handlers.</summary>
    /// <typeparam name="TMessage">The message type serialized between a named pipe client and a named pipe server.</typeparam>
    public interface INamedPipeEvents<TMessage>
    {
        /// <summary>Event raised when an exception is thrown during a read or write operation on the named pipe. The pipe's
        /// connection will be disconnected when an exception occurs.</summary>
        event EventHandler<NamedPipeExceptionEventArgs> OnException;
    }
}