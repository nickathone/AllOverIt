using Microsoft.Extensions.DependencyInjection;
using System;

namespace AllOverIt.DependencyInjection.Extensions
{
    public static partial class ServiceProviderExtensions
    {
        /// <summary>Gets a required implementation type based on a previously registered name. The implementation must implement
        /// <typeparamref name="TService"/>.</summary>
        /// <typeparam name="TService">The service type to be resolved.</typeparam>
        /// <param name="provider">The service provider used to resolve the required type.</param>
        /// <param name="name">The name identifying the implementation type to be resolved.</param>
        /// <returns>The required service implementation instance.</returns>
        public static TService GetRequiredNamedService<TService>(this IServiceProvider provider, string name) where TService : class
        {
            return provider
                .GetRequiredService<INamedServiceResolver<TService>>()
                .GetRequiredNamedService(name);

            // Could also do:
            // return provider
            //     .GetRequiredService<INamedServiceResolver>()
            //     .GetRequiredNamedService<TService>(name);
        }
    }
}