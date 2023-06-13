using AllOverIt.Serialization.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AllOverIt.Pipes.Serialization.Binary
{
    public sealed class BinaryMessageSerializer<TMessage> : MessageSerializerBase<TMessage>
    {
        public ICollection<EnrichedBinaryValueReader<TMessage>> Readers { get; } = new List<EnrichedBinaryValueReader<TMessage>>();
        public ICollection<EnrichedBinaryValueWriter<TMessage>> Writers { get; } = new List<EnrichedBinaryValueWriter<TMessage>>();

        protected override byte[] SerializeToBytes(TMessage @object)
        {
            using (var stream = new MemoryStream())
            {
                using (var serializerWriter = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    foreach (var writer in Writers)
                    {
                        serializerWriter.Writers.Add(writer);
                    }

                    serializerWriter.WriteObject(@object);
                }

                return stream.ToArray();
            }
        }

        protected override TMessage DeserializeFromBytes(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                using (var serializerReader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    foreach (var reader in Readers)
                    {
                        serializerReader.Readers.Add(reader);
                    }

                    return (TMessage) serializerReader.ReadObject();
                }
            }
        }
    }
}