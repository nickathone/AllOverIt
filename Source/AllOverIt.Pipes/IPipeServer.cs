using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes
{
    public interface IPipeServerEvents<TType>
    {
        /// <summary>
        /// Invoked whenever a client connects to the server.
        /// </summary>
        event EventHandler<ConnectionEventArgs<TType>> ClientConnected;

        /// <summary>
        /// Invoked whenever a client disconnects from the server.
        /// </summary>
        event EventHandler<ConnectionEventArgs<TType>> ClientDisconnected;
    }

    public interface IPipeServer<TType> : IPipe<TType>
    {
        /// <summary>
        /// Name of pipe
        /// </summary>
        string PipeName { get; }


        bool IsActive { get; }


        ///// <summary>
        ///// Invoked whenever a client connects to the server.
        ///// </summary>
        //event EventHandler<ConnectionEventArgs<TType>> ClientConnected;

        ///// <summary>
        ///// Invoked whenever a client disconnects from the server.
        ///// </summary>
        //event EventHandler<ConnectionEventArgs<TType>> ClientDisconnected;


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        void Start();


        ///// <summary>
        ///// Closes all open client connections and stops listening for new ones.
        ///// </summary>
        Task StopAsync();




        /// <summary>Asynchronously sends a message to the client with a specfied pipe name..</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="pipeName">The name of the pipe to send the message to. This name is case-insensitive.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        Task WriteAsync(TType message, string pipeName, CancellationToken cancellationToken = default);

        /// <summary>Asynchronously sends a message to all connected clients that meet a predicate condition.</summary>
        /// <param name="message">The message to send to all connected clients.</param>
        /// <param name="predicate">The predicate condition to be met.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        Task WriteAsync(TType message, Predicate<IPipeConnection<TType>> predicate, CancellationToken cancellationToken = default);
    }
}