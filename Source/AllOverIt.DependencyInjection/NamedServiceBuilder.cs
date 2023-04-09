using AllOverIt.Assertion;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AllOverIt.DependencyInjection
{
    internal sealed class NamedServiceBuilder<TService> : INamedServiceBuilder<TService> where TService : class
    {
        internal NamedServiceResolver<TService> _resolver = new();

        private readonly IServiceCollection _services;

        public NamedServiceBuilder(IServiceCollection services)
        {
            _services = services.WhenNotNull(nameof(services));
        }

        public INamedServiceBuilder<TService> AsTransient<TImplementation>(string name) where TImplementation : class, TService
        {
            RegisterImplementation<TImplementation>(name);
            _services.AddTransient<TService, TImplementation>();

            return this;
        }

        public INamedServiceBuilder<TService> AsTransient(string name, Type implementationType)
        {
            RegisterImplementation(name, implementationType);
            _services.AddTransient(typeof(TService), implementationType);

            return this;
        }

        public INamedServiceBuilder<TService> AsScoped<TImplementation>(string name) where TImplementation : class, TService
        {
            RegisterImplementation<TImplementation>(name);
            _services.AddScoped<TService, TImplementation>();

            return this;
        }

        public INamedServiceBuilder<TService> AsScoped(string name, Type implementationType)
        {
            RegisterImplementation(name, implementationType);
            _services.AddScoped(typeof(TService), implementationType);

            return this;
        }

        public INamedServiceBuilder<TService> AsSingleton<TImplementation>(string name) where TImplementation : class, TService
        {
            RegisterImplementation<TImplementation>(name);
            _services.AddSingleton<TService, TImplementation>();

            return this;
        }

        public INamedServiceBuilder<TService> AsSingleton(string name, Type implementationType)
        {
            RegisterImplementation(name, implementationType);
            _services.AddSingleton(typeof(TService), implementationType);

            return this;
        }

        private void RegisterImplementation<TImplementation>(string name) where TImplementation : class, TService
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            ((INamedServiceRegistration<TService>)_resolver).Register<TImplementation>(name);
        }

        private void RegisterImplementation(string name, Type implementationType)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            ((INamedServiceRegistration<TService>)_resolver).Register(name, implementationType);
        }
    }
}