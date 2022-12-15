using System.Reflection;

namespace AllOverIt.Patterns.Decorator.Proxy
{
    public static class ProxyFactory
    {
        public static TDecorated CreateProxy<TDecorated, TProxy>(TDecorated decorated)
            where TProxy : ProxyDecorator<TDecorated>
        {
            object proxyInstance = DispatchProxy.Create<TDecorated, TProxy>();

            var proxyDecorator = (ProxyDecorator<TDecorated>) proxyInstance;
            proxyDecorator._decorated = decorated;

            return (TDecorated) proxyInstance;
        }
    }
}
