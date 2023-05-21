using AllOverIt.Pipes;
using AllOverIt.Serialization.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NamedPipeTypes
{
    public sealed class BinaryMessageSerializer<TType> : MessageSerializerBase<TType>
    {
        public List<EnrichedBinaryValueReader<TType>> Readers { get; } = new();
        public List<EnrichedBinaryValueWriter<TType>> Writers { get; } = new();

        protected override byte[] SerializeToBytes(TType @object)
        {
            byte[] bytes;

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

                bytes = stream.ToArray();
            }

            return bytes;
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