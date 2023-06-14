using System.Threading.Tasks;

namespace AllOverIt.Pipes.Connection
{
    /// <summary>A <see cref="IPipeConnection{TMessage}"/> that can be connected and disconnected.</summary>
    /// <typeparam name="TMessage">The message type serialized by the connection.</typeparam>
    public interface IConnectablePipeConnection<TMessage> : IPipeConnection<TMessage>
    {
        /// <summary>Utilizes an underlying pipe stream to send and receive messages.</summary>
        void Connect();

        /// <summary>Discontinues serializing messages and disposes of the underlying pipe stream.</summary>
        Task DisconnectAsync();
    }
}