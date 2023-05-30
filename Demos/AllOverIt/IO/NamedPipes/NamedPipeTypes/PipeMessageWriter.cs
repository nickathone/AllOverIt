using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;

namespace NamedPipeTypes
{
    public sealed class PipeMessageWriter : EnrichedBinaryValueWriter<PipeMessage>
    {
        public override void WriteValue(IEnrichedBinaryWriter writer, object value)
        {
            var message = (PipeMessage) value;

            writer.WriteGuid(message.Id);
            writer.WriteSafeString(message.Text);
            writer.WriteInt32(message.Child.Value);
        }
    }
}