using AllOverIt.Pipes.Named.Connection;

namespace AllOverIt.Pipes.Named.Events
{
    public sealed class NamedPipeConnectionMessageEventArgs<TMessage, TPipeConnection> : NamedPipeConnectionEventArgs<TMessage, TPipeConnection>
        where TPipeConnection : class, INamedPipeConnection<TMessage>
    {
        /// <summary>The message sent by the other end of the pipe.</summary>
        public TMessage Message { get; }

        /// <summary>Constructor.</summary>
        /// <param name="connection">The connection associated with the event.</param>
        /// <param name="message">The message associated with the event.</param>
        public NamedPipeConnectionMessageEventArgs(TPipeConnection connection, TMessage message)
            : base(connection)
        {
            Message = message;
        }
    }
}