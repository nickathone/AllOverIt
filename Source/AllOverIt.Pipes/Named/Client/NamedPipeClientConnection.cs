using AllOverIt.Assertion;
using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using AllOverIt.Pipes.Named.Serialization;
using System;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Named.Client
{
    internal sealed class NamedPipeClientConnection<TMessage> : NamedPipeConnection<TMessage>, INamedPipeClientConnection<TMessage>
    {
        /// <inheritdoc />
        public event EventHandler<NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnMessageReceived;

        /// <inheritdoc />
        public event EventHandler<NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnDisconnected;

        /// <inheritdoc />
        public event EventHandler<NamedPipeConnectionExceptionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>> OnException;

        /// <inheritdoc />
        public string ServerName { get; }

        public NamedPipeClientConnection(PipeStream stream, string pipeName, INamedPipeSerializer<TMessage> serializer, string serverName)
            : base(stream, pipeName, serializer)
        {
            ServerName = serverName.WhenNotNullOrEmpty(nameof(serverName));
        }

        protected override void DoOnMessageReceived(TMessage message)
        {
            var onMessageReceived = OnMessageReceived;

            if (onMessageReceived is not null)
            {
                var args = new NamedPipeConnectionMessageEventArgs<TMessage, INamedPipeClientConnection<TMessage>>(this, message);

                onMessageReceived.Invoke(this, args);
            }
        }

        protected override void DoOnDisconnected()
        {
            var onDisconnected = OnDisconnected;

            if (onDisconnected is not null)
            {
                var args = new NamedPipeConnectionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>(this);

                onDisconnected.Invoke(this, args);
            }
        }

        protected override void DoOnException(Exception exception)
        {
            var onException = OnException;

            if (onException is not null)
            {
                var args = new NamedPipeConnectionExceptionEventArgs<TMessage, INamedPipeClientConnection<TMessage>>(this, exception);

                onException.Invoke(this, args);
            }
        }
    }
}