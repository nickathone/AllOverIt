using AllOverIt.Assertion;
using AllOverIt.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.DependencyInjection
{
    internal sealed class NamedServiceResolver<TService> : INamedServiceRegistration<TService>,
                                                           INamedServiceResolver<TService> where TService : class
    {
        private readonly IDictionary<string, Type> _namedImplementations = new Dictionary<string, Type>();
        internal IServiceProvider _provider;
        private IEnumerable<TService> _services;

        void INamedServiceRegistration<TService>.Register<TImplementation>(string name)
        {
            ((INamedServiceRegistration<TService>)this).Register(name, typeof(TImplementation));
        }

        void INamedServiceRegistration<TService>.Register(string name, Type implementationType)
        {
            _namedImplementations.Add(name, implementationType);
        }

        TService INamedServiceResolver<TService>.GetRequiredNamedService(string name)
        {
            if (_namedImplementations.TryGetValue(name, out var implementationType))
            {
                _services ??= _provider.GetRequiredService<IEnumerable<TService>>();

                return _services.Single(service => service.GetType() == implementationType);
            }

            throw new InvalidOperationException($"No service of type {typeof(TService).GetFriendlyName()} was found for the name {name}.");
        }
    }

    internal sealed class NamedServiceResolver : INamedServiceResolver
    {
        private readonly IServiceProvider _provider;

        public NamedServiceResolver(IServiceProvider provider)
        {
            _provider = provider.WhenNotNull(nameof(provider));
        }

        public TService GetRequiredNamedService<TService>(string key)
        {
            var resolver = _provider.GetRequiredService<INamedServiceResolver<TService>>();

            return resolver.GetRequiredNamedService(key);
        }
    }
}