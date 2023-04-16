using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AllOverIt.Aspects.Interceptor
{
    /// <summary>Provides a base class for all interceptors (dispatch proxies) created via
    /// <see cref="InterceptorFactory.CreateInterceptor{TServiceType, TInterceptor}(TServiceType, Action{TInterceptor})"/>.
    /// Derived Interceptors must be public and non-sealed as they are the base class for the generated proxy.</summary>
    /// <typeparam name="TServiceType"></typeparam>
    public abstract class InterceptorBase<TServiceType> : DispatchProxy
    {
        internal TServiceType _serviceInstance;

        /// <summary>The <see cref="BeforeInvoke(MethodInfo, object[])"/> method is called before calling the decorated
        /// object's method, and ends with calling <see cref="AfterInvoke(MethodInfo, object[], InterceptorState)"/> if
        /// no exception is raised, otherwise <see cref="Faulted(MethodInfo, object[], InterceptorState, Exception)"/>
        /// is called.</summary>
        /// <param name="targetMethod">The info for the method being intercepted.</param>
        /// <param name="args">The arguments passed to the intercepted method.</param>
        /// <returns>The result of the method invoked on the decorated instance.</returns>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var state = BeforeInvoke(targetMethod, args);

            object result = default;

            try
            {
                result = targetMethod.Invoke(_serviceInstance, args);

                if (result is Task taskResult)
                {
                    taskResult.ContinueWith(task =>
                    {
                        // Haven't come across a scenario for this yet - all faults go to the catch block

                        //if (task.IsFaulted)
                        //{
                        //    Faulted(targetMethod, args, state, task.Exception);
                        //}
                        //else
                        //{
                        //    AfterInvoke(targetMethod, args, state);
                        //}

                        AfterInvoke(targetMethod, args, state);
                    }, TaskContinuationOptions.ExecuteSynchronously);
                }
                else
                {
                    AfterInvoke(targetMethod, args, state);
                }
            }
            catch (TargetInvocationException exception)
            {
                // The InnerException will never be null - it holds the underlying exception thrown by the invoked method
                var fault = exception.InnerException;

                Faulted(targetMethod, args, state, fault);

                throw fault;
            }

            return result;
        }

        /// <summary>Called before the decorated instance method is called.</summary>
        /// <param name="targetMethod">The info for the method being intercepted.</param>
        /// <param name="args">The arguments passed to the intercepted method.</param>
        /// <returns>A state object that will be passed to the <see cref="AfterInvoke(MethodInfo, object[], InterceptorState)"/>
        /// or <see cref="Faulted(MethodInfo, object[], InterceptorState, Exception)"/> method, as applicable.
        /// If no state is required then return <see cref="InterceptorState.Unit"/>.</returns>
        protected virtual InterceptorState BeforeInvoke(MethodInfo targetMethod, object[] args)
        {
            return InterceptorState.None;
        }

        /// <summary>Called when the decorated instance method invocation faults (throws an exception).</summary>
        /// <param name="targetMethod">The info for the method being intercepted.</param>
        /// <param name="args">The arguments passed to the intercepted method.</param>
        /// <param name="state">The state object returned by <see cref="BeforeInvoke(MethodInfo, object[])"/>.
        /// If the <see cref="BeforeInvoke(MethodInfo, object[])"/> method is not overriden then this will
        /// be <see cref="InterceptorState.Unit"/>.</param>
        /// <param name="exception">The exception that was thrown by the instance method.</param>
        protected virtual void Faulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
        {
        }

        /// <summary>Called after the instance method has completed execution without faulting (throwing an exception).</summary>
        /// <param name="targetMethod">The info for the method being intercepted.</param>
        /// <param name="args">The arguments passed to the intercepted method.</param>
        /// <param name="state">The state object returned by <see cref="BeforeInvoke(MethodInfo, object[])"/>.
        /// If the <see cref="BeforeInvoke(MethodInfo, object[])"/> method is not overriden then this will
        /// be <see cref="InterceptorState.Unit"/>.</param>
        protected virtual void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
        {
        }
    }
}
