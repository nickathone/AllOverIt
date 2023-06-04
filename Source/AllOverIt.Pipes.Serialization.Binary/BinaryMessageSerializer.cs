using AllOverIt.Serialization.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AllOverIt.Pipes.Serialization.Binary
{
    public sealed class BinaryMessageSerializer<TType> : MessageSerializerBase<TType>
    {
        public IList<EnrichedBinaryValueReader<TType>> Readers { get; } = new List<EnrichedBinaryValueReader<TType>>();
        public IList<EnrichedBinaryValueWriter<TType>> Writers { get; } = new List<EnrichedBinaryValueWriter<TType>>();

        protected override byte[] SerializeToBytes(TType @object)
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

        protected override TType DeserializeFromBytes(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                using (var serializerReader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    foreach (var reader in Readers)
                    {
                        serializerReader.Readers.Add(reader);
                    }

                    return (TType) serializerReader.ReadObject();
                }
            }
        }
    }
}