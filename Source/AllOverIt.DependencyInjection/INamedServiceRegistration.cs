using System;

namespace AllOverIt.DependencyInjection
{
    public interface INamedServiceRegistration<TService>
    {
        void Register<TImplementation>(string name) where TImplementation : TService;
        void Register(string name, Type implementationType);
    }
}