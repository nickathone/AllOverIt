using Microsoft.Extensions.DependencyInjection;
using System;

namespace AllOverIt.DependencyInjection
{
    /// <summary>Provides information about a registered service that could not be resolved during validation.</summary>
    public sealed class ServiceRegistrationError
    {
        /// <summary>The service descriptor for the service that could not be resolved.</summary>
        public ServiceDescriptor Service { get; init; }

        /// <summary>The exception that was raised when attempting to resolve the service.</summary>
        public Exception Exception { get; init; }
    }
}