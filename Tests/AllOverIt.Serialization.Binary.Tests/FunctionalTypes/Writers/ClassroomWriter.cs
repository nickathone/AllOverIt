using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using AllOverIt.Tests.Serialization.Binary.FunctionalTypes.Models;

namespace AllOverIt.Tests.Serialization.Binary.FunctionalTypes.Writers
{
    internal sealed class ClassroomWriter : EnrichedBinaryValueWriter<Classroom>
    {
        public override void WriteValue(IEnrichedBinaryWriter writer, object value)
        {
            var classroom = (Classroom) value;

            writer.WriteGuid(classroom.RoomId);
            writer.WriteObject(classroom.Teacher);
            writer.WriteEnumerable(classroom.Students);
        }
    }
}
