using System;

namespace AllOverIt.Serialization.Binary.Writers
{
    /// <inheritdoc cref="IEnrichedBinaryValueWriter"/>
    public abstract class EnrichedBinaryValueWriter : IEnrichedBinaryValueWriter
    {
        /// <inheritdoc />
        public Type Type { get; }

        public EnrichedBinaryValueWriter(Type type)
        {
            Type = type;
        }

        /// <inheritdoc />
        public abstract void WriteValue(IEnrichedBinaryWriter writer, object value);
    }
}
