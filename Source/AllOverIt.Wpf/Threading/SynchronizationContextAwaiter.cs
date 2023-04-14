using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AllOverIt.Wpf.Threading
{
    /// <summary>An awaiter that will queue a continuation on the thread associated with
    /// a SynchronizationContext.</summary>
    public readonly struct SynchronizationContextAwaiter : INotifyCompletion
    {
        // The 'state' is the continuation to be invoked
        private static readonly SendOrPostCallback SynchronizationCallback = state => ((Action) state).Invoke();

        private readonly SynchronizationContext _synchronizationContext;

        /// <summary>Indicates if the required SynchronizationContext is the same as the current thread's
        /// SynchronizationContext.</summary>
        public bool IsCompleted => _synchronizationContext == SynchronizationContext.Current;

        internal SynchronizationContextAwaiter(SynchronizationContext synchronizationContext)
        {
            _synchronizationContext = synchronizationContext;
        }

        /// <summary>Queues the <paramref name="continuation"/> on the thread associated with the required
        /// SynchronizationContext.</summary>
        /// <param name="continuation">The continuation to be queued on the thread associated with the required SynchronizationContext.</param>
        public void OnCompleted(Action continuation)
        {
            _synchronizationContext.Post(SynchronizationCallback, continuation);
        }

        /// <summary>Performs no operation. A required method for the awaiter.</summary>
        public void GetResult()
        {
        }
    }
}