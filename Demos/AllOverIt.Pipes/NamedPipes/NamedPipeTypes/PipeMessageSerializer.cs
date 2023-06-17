using AllOverIt.Pipes.Serialization.Binary;

namespace NamedPipeTypes
{
    public sealed class PipeMessageSerializer : BinaryMessageSerializer<PipeMessage>
    {
        public PipeMessageSerializer()
        {
            Readers.Add(new PipeMessageReader());
            Writers.Add(new PipeMessageWriter());
        }
    }
}