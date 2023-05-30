using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;

namespace NamedPipeTypes
{
    public sealed class PipeMessageReader : EnrichedBinaryValueReader<PipeMessage>
    {
        public override object ReadValue(IEnrichedBinaryReader reader)
        {
            var id = reader.ReadGuid();
            var text = reader.ReadSafeString();
            var value = reader.ReadInt32();

            return new PipeMessage
            {
                Id = id,
                Text = text,
                Child =
                {
                    Value = value
                }
            };
        }
    }
}