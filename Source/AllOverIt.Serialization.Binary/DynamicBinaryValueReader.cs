using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System;
using System.Linq;

namespace AllOverIt.Serialization.Binary
{
    public class DynamicBinaryValueReader : EnrichedBinaryValueReader
    {
        public DynamicBinaryValueReader(Type type)
            : base(type)
        {
        }

        public override object ReadValue(IEnrichedBinaryReader reader)
        {
            _ = reader.WhenNotNull(nameof(reader));

            // Create an instance of the required type
            var instance = Activator.CreateInstance(Type);

            var properties = Type
               .GetPropertyInfo(/*Options.BindingOptions*/)
               .Where(propInfo => propInfo.CanRead && propInfo.CanWrite && !propInfo.IsIndexer());

            foreach (var propertyInfo in properties)
            {
                // Read the property value - ReadObject() caters for class types by invoking a custom value
                // reader or creating a new (or a cached) DynamicBinaryValueReader for the class type.
                var value = reader.ReadObject();

                propertyInfo.SetValue(instance, value);
            }

            return instance;
        }
    }
}
