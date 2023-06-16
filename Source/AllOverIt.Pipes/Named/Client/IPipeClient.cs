using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Named.Client
{
    public interface IPipeClient<TMessage> : IPipe<TMessage>, IPipeEvents<TMessage>, IPipeClientEvents<TMessage>, IAsyncDisposable
    {
        /// <summary>The name of pipe.</summary>
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
        /// Connects to the named pipe server asynchronously.
        /// </summary>
        /// <returns></returns>
        Task ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Disconnects from server
        /// </summary>
        /// <returns></returns>
        Task DisconnectAsync(CancellationToken cancellationToken = default);
    }
}