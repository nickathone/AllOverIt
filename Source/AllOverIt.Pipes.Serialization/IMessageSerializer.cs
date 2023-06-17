namespace AllOverIt.Pipes.Serialization
{
    public interface IMessageSerializer<TMessage>
    {
        /// <summary>Serializes a <typeparamref name="TMessage"/> to a byte array.</summary>
        /// <param name="message">The message to be serialized.</param>
        /// <returns>The message serialized to a byte array.</returns>
        public byte[] Serialize(TMessage message);

        /// <summary>Deserializes a byte array to a <typeparamref name="TMessage"/> instance.</summary>
        /// <typeparam name="TType">The message type to create from the array of bytes.</typeparam>
        /// <param name="bytes">The serialized byte array.</param>
        /// <returns>An initialized instance of the required type.</returns>
        public TMessage Deserialize(byte[] bytes);
    }
}