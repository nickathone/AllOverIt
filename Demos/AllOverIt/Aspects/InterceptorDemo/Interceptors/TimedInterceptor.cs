using AllOverIt.Aspects.Interceptor;
using AllOverIt.Extensions;
using ServiceProxyDemo;
using System;
using System.Diagnostics;
using System.Reflection;

namespace InterceptorDemo.Interceptors
{
    internal class TimedInterceptor : InterceptorBase<ISecretService>
    {
        public long? MinimimReportableMilliseconds { get; set; }

        private class TimedState : InterceptorState
        {
            public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
        }

        protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, object[] args)
        {
            _ = base.BeforeInvoke(targetMethod, args);

            Console.WriteLine($"Before {targetMethod.Name}");

            // Would return ProxyState.None if no state is required
            return new TimedState();
        }

        protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
        {
            base.AfterInvoke(targetMethod, args, state);

            Console.WriteLine($"After {targetMethod.Name}");

            CheckElapsedPeriod(state);
        }

        protected override void Faulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
        {
            base.Faulted(targetMethod, args, state, exception);

            Console.WriteLine($"FAULTED {targetMethod.Name} : {exception.GetType().GetFriendlyName()} - {exception.Message}");

            CheckElapsedPeriod(state);
        }

        private void CheckElapsedPeriod(InterceptorState state)
        {
            var timedState = state as TimedState;
            var elapsed = timedState.Stopwatch.ElapsedMilliseconds;

            if (!MinimimReportableMilliseconds.HasValue || elapsed >= MinimimReportableMilliseconds)
            {
                if (MinimimReportableMilliseconds.HasValue)
                {
                    Console.WriteLine($" >> WARNING: Elapsed exceeded minimum {MinimimReportableMilliseconds.Value}ms, actual = {elapsed}ms");
                }
                else
                {
                    Console.WriteLine($" >> Elapsed = {elapsed}ms");
                }
            }
        }
    }
}