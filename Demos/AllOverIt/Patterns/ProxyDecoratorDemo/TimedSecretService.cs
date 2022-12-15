using AllOverIt.Extensions;
using AllOverIt.Patterns.Decorator.Proxy;
using System;
using System.Diagnostics;
using System.Reflection;

namespace ProxyDecoratorDemo
{
    internal class TimedSecretService : ProxyDecorator<ISecretService>
    {
        private class TimedState : ProxyState
        {
            public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
        }

        protected override ProxyState BeforeInvoke(MethodInfo targetMethod, object[] args)
        {
            _ = base.BeforeInvoke(targetMethod, args);

            Console.WriteLine($"Before {targetMethod.Name}");

            // Would return ProxyState.None if no state is required
            return new TimedState();
        }

        protected override void AfterInvoke(MethodInfo targetMethod, object[] args, ProxyState state)
        {
            base.AfterInvoke(targetMethod, args, state);

            var timedState = state as TimedState;

            Console.WriteLine($"After {targetMethod.Name} - {timedState.Stopwatch.ElapsedMilliseconds}ms");
        }

        protected override void Faulted(MethodInfo targetMethod, object[] args, ProxyState state, Exception exception)
        {
            base.Faulted(targetMethod, args, state, exception);

            var timedState = state as TimedState;

            Console.WriteLine($"FAULTED {targetMethod.Name} after {timedState.Stopwatch.ElapsedMilliseconds}ms: {exception.GetType().GetFriendlyName()} - {exception.Message}");
        }
    }
}