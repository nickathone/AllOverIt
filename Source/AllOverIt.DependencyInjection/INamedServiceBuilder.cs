using System;

namespace AllOverIt.DependencyInjection
{
    public interface INamedServiceBuilder<TService> where TService : class
    {
        INamedServiceBuilder<TService> AsTransient<TImplementation>(string name) where TImplementation : class, TService;
        INamedServiceBuilder<TService> AsTransient(string name, Type implementationType);

        INamedServiceBuilder<TService> AsScoped<TImplementation>(string name) where TImplementation : class, TService;
        INamedServiceBuilder<TService> AsScoped(string name, Type implementationType);

        INamedServiceBuilder<TService> AsSingleton<TImplementation>(string name) where TImplementation : class, TService;
        INamedServiceBuilder<TService> AsSingleton(string name, Type implementationType);
    }
}