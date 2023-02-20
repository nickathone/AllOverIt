using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.Aspects.Interceptor;
using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>Decorates all registered <typeparamref name="TServiceType"/> types with <typeparamref name="TDecoratorType"/> types. Decoration is only applied
        /// to services already registered at the time of making the call.</summary>
        /// <typeparam name="TServiceType">The service type to be decorated.</typeparam>
        /// <typeparam name="TDecoratorType">The type decorating the service type. This type's constructor is expected to accept an argument of type <typeparamref name="TServiceType"/>
        /// in addition to any other dependencies it requires.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection Decorate<TServiceType, TDecoratorType>(this IServiceCollection services)
            where TDecoratorType : TServiceType
        {
            ReplaceServiceDescriptor<TServiceType>(services, descriptor => Decorate(descriptor, typeof(TDecoratorType)));

            return services;
        }

        public static IServiceCollection DecorateWithInterceptor<TServiceType, TInterceptor>(this IServiceCollection services, Action<TInterceptor> configure = default)
            where TInterceptor : InterceptorBase<TServiceType>
        {
            ReplaceServiceDescriptor<TServiceType>(services, descriptor => DecorateWithInterceptor<TServiceType, TInterceptor>(descriptor, configure));

            return services;
        }

        private static void ReplaceServiceDescriptor<TServiceType>(IServiceCollection services, Func<ServiceDescriptor, ServiceDescriptor> descriptorResolver)
        {
            var descriptors = GetServiceDescriptors<TServiceType>(services);

            foreach (var descriptor in descriptors)
            {
                var index = services.IndexOf(descriptor);
                services[index] = descriptorResolver.Invoke(descriptor);
            }
        }

        private static ServiceDescriptor Decorate(ServiceDescriptor descriptor, Type decoratorType)
        {
            return ServiceDescriptor.Describe(
                descriptor.ServiceType,
                provider =>
                {
                    var instance = GetInstance(provider, descriptor);
                    return ActivatorUtilities.CreateInstance(provider, decoratorType, instance);
                },
                descriptor.Lifetime);
        }

        private static ServiceDescriptor DecorateWithInterceptor<TServiceType, TInterceptor>(ServiceDescriptor descriptor, Action<TInterceptor> configure)
            where TInterceptor : InterceptorBase<TServiceType>
        {
            return ServiceDescriptor.Describe(
                descriptor.ServiceType,
                provider =>
                {
                    var instance = (TServiceType) GetInstance(provider, descriptor);
                    return InterceptorFactory.CreateInterceptor<TServiceType, TInterceptor>(instance, configure);
                },
                descriptor.Lifetime);
        }

        private static IReadOnlyCollection<ServiceDescriptor> GetServiceDescriptors<TServiceType>(IServiceCollection services)
        {
            var serviceType = typeof(TServiceType);
            var descriptors = services.Where(service => service.ServiceType == serviceType).AsReadOnlyCollection();

            if (!descriptors.Any())
            {
                throw new DependencyRegistrationException($"No registered services found for the type '{serviceType.GetFriendlyName()}'.");
            }

            return descriptors;
        }

        private static object GetInstance(IServiceProvider provider, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance;
            }

            return descriptor.ImplementationType != null
                ? ActivatorUtilities.GetServiceOrCreateInstance(provider, descriptor.ImplementationType)
                : descriptor.ImplementationFactory!.Invoke(provider);
        }
    }
}