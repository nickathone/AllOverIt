using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Named.Client
{
    /// <summary>Represents a named pipe client that can serialize messages of type <typeparamref name="TMessage"/> with a named pipe server.</summary>
    /// <typeparam name="TMessage">The message type serialized between a named pipe client and a named pipe server.</typeparam>
    public interface INamedPipeClient<TMessage> : INamedPipe<TMessage>, INamedPipeEvents<TMessage>, INamedPipeClientEvents<TMessage>, IAsyncDisposable
    {
        /// <summary>The name of pipe.</summary>
        public string PipeName { get; }

        /// <summary>Indicates if the connection to a named pipe server is connected.</summary>
        bool IsConnected { get; }

        /// <summary>The name of the server the pipe client is connected to. A dot '.' indicates the local machine.</summary>
        public string ServerName { get; }

        /// <summary>Asynchronously connects to the named pipe server.</summary>
        /// <returns>A task that completes when the connection is complete.</returns>
        Task ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary>Asynchronously disconnects from the named pipe server.</summary>
        /// <returns>A task that completes when the disconnection is complete.</returns>
        Task DisconnectAsync(CancellationToken cancellationToken = default);
    }
}