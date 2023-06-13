using AllOverIt.Pipes.Connection;

namespace AllOverIt.Pipes.Events
{
    public sealed class ConnectionMessageEventArgs<TMessage, TPipeConnection> : ConnectionEventArgs<TMessage, TPipeConnection>
        where TPipeConnection : class, IPipeConnection<TMessage>
    {
        /// <summary>The message sent by the other end of the pipe.</summary>
        public TMessage Message { get; }

        /// <summary>Constructor.</summary>
        /// <param name="connection">The connection associated with the event.</param>
        /// <param name="message">The message associated with the event.</param>
        public ConnectionMessageEventArgs(TPipeConnection connection, TMessage message)
            : base(connection)
        {
            Message = message;
        }
    }
}