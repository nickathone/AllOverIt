using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using AllOverIt.Serialization.Binary.Tests.FunctionalTypes.Models;

namespace AllOverIt.Serialization.Binary.Tests.FunctionalTypes.Writers
{
    internal abstract class PersonWriter<TPerson> : EnrichedBinaryValueWriter<TPerson> where TPerson : Person
    {
        public override void WriteValue(IEnrichedBinaryWriter writer, object value)
        {
            var person = (Person) value;

            writer.WriteSafeString(person.FirstName);
            writer.WriteSafeString(person.LastName);
            writer.WriteEnum(person.Gender);                // writes the type and value
            writer.WriteNullable(person.Age);               // caters for nullable values
        }
    }
}
