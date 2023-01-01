using AllOverIt.Assertion;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace AllOverIt.DependencyInjection.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>Validates all registered services to ensure their dependencies are also registered.</summary>
        /// <param name="services">The service collection to be validated.</param>
        /// <param name="provider">The service provider built for the collection of registered services.</param>
        /// <returns>A <see cref="ServiceRegistrationError"/> for each error found.</returns>
        /// <remarks>This method should only be used during development as it is an expensive operation.</remarks>
        public static IReadOnlyCollection<ServiceRegistrationError> Validate(this IServiceCollection services, IServiceProvider provider)
        {
            _ = services.WhenNotNull(nameof(services));
            _ = provider.WhenNotNull(nameof(provider));

            var errors = new List<ServiceRegistrationError>();

            foreach (var service in services)
            {
                // Skip generic services as we don't know how to deal with them
                if (service.ServiceType.ContainsGenericParameters)
                {
                    continue;
                }

                ServiceRegistrationError error = null;

                try
                {
                    _ = provider.GetRequiredService(service.ServiceType);
                }
                catch(Exception exception)
                {
                    error = new ServiceRegistrationError
                    {
                        Service = service,
                        Exception = exception
                    };
                }

                if (error is not null)
                {
                    errors.Add(error);
                }
            }

            return errors;
        }
    }
}