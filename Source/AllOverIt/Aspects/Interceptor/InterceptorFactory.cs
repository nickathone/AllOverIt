using System;
using System.Reflection;

namespace AllOverIt.Aspects.Interceptor
{
    public static class InterceptorFactory
    {
        // TODO: An overload to new() TDecorated when not provided
        public static TDecorated CreateProxy<TDecorated, TInterceptor>(TDecorated decorated, Action<TInterceptor> configure = default)
            where TInterceptor : InterceptorBase<TDecorated>
        {
            object proxyInstance = DispatchProxy.Create<TDecorated, TInterceptor>();

            var proxyDecorator = (InterceptorBase<TDecorated>) proxyInstance;
            proxyDecorator._decorated = decorated;

            if (configure is not null)
            {
                var interceptor = (TInterceptor) proxyInstance;
                configure.Invoke(interceptor);
            }

            return (TDecorated) proxyInstance;
        }
    }
}
