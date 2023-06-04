using AllOverIt.Pipes.Serialization;

namespace NamedPipeTypes
{
    public sealed class PipeMessageSerializer : IMessageSerializer<PipeMessage>
    {
        private readonly BinaryMessageSerializer<PipeMessage> _serializer;   // Inherits IMessageSerializer<PipeMessage>

        public PipeMessageSerializer()
        {
            _serializer = new BinaryMessageSerializer<PipeMessage>();

            _serializer.Readers.Add(new PipeMessageReader());
            _serializer.Writers.Add(new PipeMessageWriter());
        }

        public PipeMessage Deserialize(byte[] bytes)
        {
            return _serializer.Deserialize(bytes);
        }

        public byte[] Serialize(PipeMessage @object)
        {
            return _serializer.Serialize(@object);
        }
    }
}