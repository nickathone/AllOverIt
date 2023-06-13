namespace AllOverIt.Pipes.Serialization
{
    public interface IMessageSerializer<TMessage>
    {
        /// <summary>Serializes an object to a byte array.</summary>
        /// <param name="object">The object to be serialized.</param>
        /// <returns>The object serialized to a byte array.</returns>
        public byte[] Serialize(TMessage @object);

        /// <summary>Deserializes a byte array to an object instance.</summary>
        /// <typeparam name="TType">The object type to create from the array of bytes.</typeparam>
        /// <param name="bytes">The serialized byte array.</param>
        /// <returns>An initialized instance of the required type.</returns>
        public TMessage Deserialize(byte[] bytes);
    }
}