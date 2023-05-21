namespace AllOverIt.Pipes
{
    public class ConnectionMessageEventArgs<TType> : ConnectionEventArgs<TType>
    {
        /// <summary>
        /// Message sent by the other end of the pipe
        /// </summary>
        public TType Message { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="message"></param>
        public ConnectionMessageEventArgs(IPipeConnection<TType> connection, TType message)
            : base(connection)
        {
            Message = message;
        }
    }


}