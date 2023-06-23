using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AllOverIt.Pipes.Named.Serialization
{
    /// <summary>Provides binary serialization support for a named pipe. Reading is performed using <see cref="EnrichedBinaryValueReader"/>
    /// and writing is performed using <see cref="EnrichedBinaryValueWriter"/>. If a custom reader or writer is not provided for any property
    /// type (or even <typeparamref name="TMessage"/>) then a <see cref="DynamicBinaryValueReader"/> or <see cref="DynamicBinaryValueWriter"/> will be
    /// used as required.</summary>
    /// <typeparam name="TMessage">The message type to be serialized.</typeparam>
    public class NamedPipeSerializer<TMessage> : INamedPipeSerializer<TMessage>
    {
        /// <summary>Contains custom value readers for <typeparamref name="TMessage"/> or any of its' property types. If the serializer
        /// encounters a type for which there is no <see cref="IEnrichedBinaryValueReader"/> then a <see cref="DynamicBinaryValueReader"/>
        /// will be used.</summary>
        public ICollection<IEnrichedBinaryValueReader> Readers { get; } = new List<IEnrichedBinaryValueReader>();

        /// <summary>Contains custom value writer for <typeparamref name="TMessage"/> or any of its' property types. If the serializer
        /// encounters a type for which there is no <see cref="IEnrichedBinaryValueWriter"/> then a <see cref="DynamicBinaryValueWriter"/>
        /// will be used.</summary>
        public ICollection<IEnrichedBinaryValueWriter> Writers { get; } = new List<IEnrichedBinaryValueWriter>();

        /// <inheritdoc/>
        public byte[] Serialize(TMessage message)
        {
            if (message == null)
            {
                return Array.Empty<byte>();
            }

            using (var stream = new MemoryStream())
            {
                using (var serializerWriter = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    foreach (var writer in Writers)
                    {
                        serializerWriter.Writers.Add(writer);
                    }

                    serializerWriter.WriteObject(message, typeof(TMessage));
                }

                return stream.ToArray();
            }
        }

        /// <inheritdoc/>
        public TMessage Deserialize(byte[] bytes)
        {
            if (bytes == null || !bytes.Any())
            {
                return default;
            }

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