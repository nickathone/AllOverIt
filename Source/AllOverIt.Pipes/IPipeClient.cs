using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes
{
    public interface IPipeClientEvents<TType>
    {
        /// <summary>
        /// Invoked after each the client connect to the server (include reconnects).
        /// </summary>
        event EventHandler<ConnectionEventArgs<TType>> OnConnected;

        /// <summary>
        /// Invoked when the client disconnects from the server (e.g., the pipe is closed or broken).
        /// </summary>
        event EventHandler<ConnectionEventArgs<TType>> OnDisconnected;
    }

    public interface IPipeClient<TType> : IPipe<TType>
    {

        /// <summary>
        /// Used pipe name.
        /// </summary>
        public string PipeName { get; }


        /// <summary>
        /// Checks that connection is exists.
        /// </summary>
        bool IsConnected { get; }


        /// <summary>
        /// Used server name.
        /// </summary>
        public string ServerName { get; }

        /// <summary>
        /// Active connection.
        /// </summary>
        //public PipeConnection<T> Connection { get; }


        ///// <summary>
        ///// Invoked after each the client connect to the server (include reconnects).
        ///// </summary>
        //event EventHandler<ConnectionEventArgs<T>> OnConnected;

        ///// <summary>
        ///// Invoked when the client disconnects from the server (e.g., the pipe is closed or broken).
        ///// </summary>
        //event EventHandler<ConnectionEventArgs<T>> OnDisconnected;


        /// <summary>
        /// Connects to the named pipe server asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        Task ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Disconnects from server
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        Task DisconnectAsync(CancellationToken cancellationToken = default);

    }
}