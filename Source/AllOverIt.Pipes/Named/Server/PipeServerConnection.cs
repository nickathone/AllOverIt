using AllOverIt.Pipes.Exceptions;
using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Named.Serialization;
using System;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Named.Server
{
    internal sealed class PipeServerConnection<TMessage> : PipeConnection<TMessage>, IPipeServerConnection<TMessage>
    {
        /// <inheritdoc />
        public event EventHandler<ConnectionMessageEventArgs<TMessage, IPipeServerConnection<TMessage>>> OnMessageReceived;

        /// <inheritdoc />
        public event EventHandler<ConnectionEventArgs<TMessage, IPipeServerConnection<TMessage>>> OnDisconnected;

        /// <inheritdoc />
        public event EventHandler<ConnectionExceptionEventArgs<TMessage, IPipeServerConnection<TMessage>>> OnException;

        public PipeServerConnection(PipeStream stream, string pipeName, IPipeSerializer<TMessage> serializer)
            : base(stream, pipeName, serializer)
        {
        }

        /// <inheritdoc />
        public string GetImpersonationUserName()
        {
            if (PipeStream is not NamedPipeServerStream serverStream)
            {
                throw new PipeConnectionException($"The pipe stream is not a {nameof(NamedPipeServerStream)}.");
            }

            // IOException will be raised of the pipe connection has been broken or
            // the user name is longer than 19 characters.
            return serverStream.GetImpersonationUserName();
        }

        protected override void DoOnMessageReceived(TMessage message)
        {
            var onMessageReceived = OnMessageReceived;

            if (onMessageReceived is not null)
            {
                var args = new ConnectionMessageEventArgs<TMessage, IPipeServerConnection<TMessage>>(this, message);

                onMessageReceived.Invoke(this, args);
            }
        }

        protected override void DoOnDisconnected()
        {
            var onDisconnected = OnDisconnected;

            if (onDisconnected is not null)
            {
                var args = new ConnectionEventArgs<TMessage, IPipeServerConnection<TMessage>>(this);

                onDisconnected.Invoke(this, args);
            }
        }

        protected override void DoOnException(Exception exception)
        {
            var onException = OnException;

            if (onException is not null)
            {
                var args = new ConnectionExceptionEventArgs<TMessage, IPipeServerConnection<TMessage>>(this, exception);

                onException.Invoke(this, args);
            }
        }
    }
}