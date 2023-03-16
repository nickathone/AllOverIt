using Microsoft.Extensions.DependencyInjection;
using System;

namespace AllOverIt.DependencyInjection.Extensions
{
    public static partial class ServiceProviderExtensions
    {
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