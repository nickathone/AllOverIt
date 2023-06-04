using System;
using System.Linq;

namespace AllOverIt.Pipes.Serialization
{
    public abstract class MessageSerializerBase<TType> : IMessageSerializer<TType>
    {
        /// <inheritdoc/>
        public byte[] Serialize(TType @object)
        {
            if (@object == null)
            {
                return Array.Empty<byte>();
            }

            return SerializeToBytes(@object);
        }

        /// <inheritdoc/>
        public TType Deserialize(byte[] bytes)
        {
            if (bytes == null || !bytes.Any())
            {
                return default;
            }

            return DeserializeFromBytes(bytes);
        }

        protected abstract byte[] SerializeToBytes(TType obj);
        protected abstract TType DeserializeFromBytes(byte[] bytes);
    }
}