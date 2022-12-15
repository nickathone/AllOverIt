using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AllOverIt.Aspects.Interceptor
{
    public abstract class InterceptorState
    {
        private class Unit : InterceptorState
        {
        }

        public static InterceptorState None => new Unit();
    }

    public abstract class InterceptorBase<TDecorated> : DispatchProxy
    {
        internal TDecorated _decorated;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var state = BeforeInvoke(targetMethod, args);

            object result = default;

            try
            {
                result = targetMethod.Invoke(_decorated, args);

                if (result is Task taskResult)
                {
                    taskResult.ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Faulted(targetMethod, args, state, task.Exception);
                        }
                        else
                        {
                            AfterInvoke(targetMethod, args, state);
                        }
                    }, TaskContinuationOptions.ExecuteSynchronously);
                }
                else
                {
                    AfterInvoke(targetMethod, args, state);
                }
            }
            catch (TargetInvocationException exception)
            {
                var fault = exception.InnerException ?? exception;

                Faulted(targetMethod, args, state, fault);

                throw fault;
            }

            return result;
        }

        protected virtual InterceptorState BeforeInvoke(MethodInfo targetMethod, object[] args)
        {
            return InterceptorState.None;
        }

        protected virtual void Faulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
        {
        }

        protected virtual void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
        {
        }
    }
}
