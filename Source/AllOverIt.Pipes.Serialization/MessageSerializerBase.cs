using System;
using System.Linq;

namespace AllOverIt.Pipes.Serialization
{
    public abstract class MessageSerializerBase<TMessage> : IMessageSerializer<TMessage>
    {
        /// <inheritdoc/>
        public byte[] Serialize(TMessage @object)
        {
            if (@object == null)
            {
                return Array.Empty<byte>();
            }

            return SerializeToBytes(@object);
        }

        /// <inheritdoc/>
        public TMessage Deserialize(byte[] bytes)
        {
            if (bytes == null || !bytes.Any())
            {
                return default;
            }

            return DeserializeFromBytes(bytes);
        }

        protected abstract byte[] SerializeToBytes(TMessage obj);
        protected abstract TMessage DeserializeFromBytes(byte[] bytes);
    }
}