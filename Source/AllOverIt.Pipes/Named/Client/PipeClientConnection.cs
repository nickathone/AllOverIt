using AllOverIt.Assertion;
using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Named.Serialization;
using System;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Named.Client
{
    internal sealed class PipeClientConnection<TMessage> : PipeConnection<TMessage>, IPipeClientConnection<TMessage>
    {
        /// <inheritdoc />
        public event EventHandler<ConnectionMessageEventArgs<TMessage, IPipeClientConnection<TMessage>>> OnMessageReceived;

        /// <inheritdoc />
        public event EventHandler<ConnectionEventArgs<TMessage, IPipeClientConnection<TMessage>>> OnDisconnected;

        /// <inheritdoc />
        public event EventHandler<ConnectionExceptionEventArgs<TMessage, IPipeClientConnection<TMessage>>> OnException;

        /// <inheritdoc />
        public string ServerName { get; }

        public PipeClientConnection(PipeStream stream, string pipeName, IPipeSerializer<TMessage> serializer, string serverName)
            : base(stream, pipeName, serializer)
        {
            ServerName = serverName.WhenNotNullOrEmpty(nameof(serverName));
        }

        protected override void DoOnMessageReceived(TMessage message)
        {
            var onMessageReceived = OnMessageReceived;

            if (onMessageReceived is not null)
            {
                var args = new ConnectionMessageEventArgs<TMessage, IPipeClientConnection<TMessage>>(this, message);

                onMessageReceived.Invoke(this, args);
            }
        }

        protected override void DoOnDisconnected()
        {
            var onDisconnected = OnDisconnected;

            if (onDisconnected is not null)
            {
                var args = new ConnectionEventArgs<TMessage, IPipeClientConnection<TMessage>>(this);

                onDisconnected.Invoke(this, args);
            }
        }

        protected override void DoOnException(Exception exception)
        {
            var onException = OnException;

            if (onException is not null)
            {
                var args = new ConnectionExceptionEventArgs<TMessage, IPipeClientConnection<TMessage>>(this, exception);

                onException.Invoke(this, args);
            }
        }
    }
}