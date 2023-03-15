using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static INamedServiceBuilder<TService> AddNamedServices<TService>(this IServiceCollection services) where TService : class
        {
            // The builder provides a mechanism for the caller to register named services in the IServiceCollection and an INamedDependencyResolver
            var builder = new NamedServiceBuilder<TService>(services);

            // Register the resolver that the caller will ultimately use for resolving a previously registered service
            services.AddSingleton<INamedServiceResolver<TService>>(provider =>
            {
                var resolver = builder._resolver;

                // We can now inject the provider so the resolver can resolve from the IServiceProvider
                resolver._provider = provider;

                return resolver;
            });

            services.AddSingleton<INamedServiceResolver>(provider => new NamedServiceResolver(provider));

            return builder;
        }
    }
}