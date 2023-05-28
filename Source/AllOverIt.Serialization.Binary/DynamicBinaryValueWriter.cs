using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System;
using System.Linq;

namespace AllOverIt.Serialization.Binary
{
    public class DynamicBinaryValueWriter : EnrichedBinaryValueWriter
    {
        public DynamicBinaryValueWriter(Type type)
            : base(type)
        {
        }

        public override void WriteValue(IEnrichedBinaryWriter writer, object instance)
        {
            _ = writer.WhenNotNull(nameof(writer));
            _ = instance.WhenNotNull(nameof(instance));

            var properties = Type
                .GetPropertyInfo(/*Options.BindingOptions*/)
                .Where(propInfo => propInfo.CanRead &&
                                   !propInfo.IsIndexer());

            foreach (var propertyInfo in properties)
            {
                var propertyType = propertyInfo.PropertyType;
                var value = propertyInfo.GetValue(instance);

                // Write the property value - WriteObject() caters for class types by invoking a custom value
                // writer or creating a new (or a cached) DynamicBinaryValueWriter for the class type.
                writer.WriteObject(value, propertyInfo.PropertyType);
            }
        }
    }
}
