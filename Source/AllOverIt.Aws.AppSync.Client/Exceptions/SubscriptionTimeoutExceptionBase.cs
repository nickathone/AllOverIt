using System;
using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    /// <summary>Base class for all subscription related timeout exceptions.</summary>
    public abstract class SubscriptionTimeoutExceptionBase : TimeoutExceptionBase
    {
        /// <summary>The unique Id for the subscription that timed out.</summary>
        public string Id { get; }

        /// <summary>Constructor.</summary>
        /// <param name="id">The unique Id for the subscription that timed out.</param>
        /// <param name="timeout">The timeout period that expired.</param>
        protected SubscriptionTimeoutExceptionBase(string id, TimeSpan timeout)
            : base(timeout)
        {
            Id = id;
        }

        /// <summary>Constructor required for serialization.</summary>
        /// <param name="info">Stores the data required for serialization and deserialization.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream.</param>
        protected SubscriptionTimeoutExceptionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Id = info.GetString("Id");
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);

            base.GetObjectData(info, context);
        }
    }
}