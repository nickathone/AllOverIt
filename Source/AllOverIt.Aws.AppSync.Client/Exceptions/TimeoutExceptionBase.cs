﻿using System;
using System.Runtime.Serialization;

namespace AllOverIt.Aws.AppSync.Client.Exceptions
{
    /// <summary>A base class for all graphql related timeout based exceptions.</summary>
    public abstract class TimeoutExceptionBase : Exception
    {
        /// <summary>The timeout period that expired.</summary>
        public TimeSpan Timeout { get; }

        /// <summary>Constructor.</summary>
        /// <param name="timeout">The timeout period that expired.</param>
        protected TimeoutExceptionBase(TimeSpan timeout)
        {
            Timeout = timeout;
        }

        /// <summary>Constructor required for serialization.</summary>
        /// <param name="info">Stores the data required for serialization and deserialization.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream.</param>
        protected TimeoutExceptionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Timeout = (TimeSpan) info.GetValue("Timeout", typeof(TimeSpan))!;
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Timeout", Timeout);

            base.GetObjectData(info, context);
        }
    }
}