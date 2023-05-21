namespace AllOverIt.Pipes
{
    public sealed class ConnectionMessageEventArgs<TType> : ConnectionEventArgs<TType>
    {
        /// <summary>The message sent by the other end of the pipe.</summary>
        public TType Message { get; }

        /// <summary>Constructor.</summary>
        /// <param name="connection">The connection associated with the event.</param>
        /// <param name="message">The message associated with the event.</param>
        public ConnectionMessageEventArgs(IPipeConnection<TType> connection, TType message)
            : base(connection)
        {
            Message = message;
        }
    }
}