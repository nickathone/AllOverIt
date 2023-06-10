﻿using AllOverIt.Pipes.Connection;
using AllOverIt.Pipes.Events;
using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pipes.Server
{
    /// <summary>Represents a named pipe server that can broadcast a strongly type message to all connected clients
    /// as well as receive messages from those clients.</summary>
    /// <typeparam name="TType"></typeparam>
    public interface IPipeServer<TType> : IPipe<TType>, IPipeEvents<TType>, IPipeServerEvents<TType>
    {
        /// <summary>The name of the pipe.</summary>
        string PipeName { get; }

        /// <summary>Indicates if the server
        /// 
        /// </summary>
        bool IsActive { get; }


        void Start(Action<PipeSecurity> securityConfiguration);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        void Start(PipeSecurity pipeSecurity = null);


        /// <summary>Closes all client connections and stops listening for new connections.</summary>
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