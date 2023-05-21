using AllOverIt.Assertion;
using System;

namespace AllOverIt.Pipes
{
    public class ConnectionEventArgs<TType> : EventArgs
    {
        /// <summary>
        /// Connection
        /// </summary>
        public IPipeConnection<TType> Connection { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public ConnectionEventArgs(IPipeConnection<TType> connection)
        {
            Connection = connection.WhenNotNull(nameof(connection));
        }
    }


}