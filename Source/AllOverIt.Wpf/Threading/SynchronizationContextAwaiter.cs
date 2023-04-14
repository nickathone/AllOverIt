using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AllOverIt.Wpf.Threading
{
    public readonly struct SynchronizationContextAwaiter : INotifyCompletion
    {
        private static readonly SendOrPostCallback PostCallback = state => ((Action) state).Invoke();

        private readonly SynchronizationContext _context;

        public bool IsCompleted => _context == SynchronizationContext.Current;

        internal SynchronizationContextAwaiter(SynchronizationContext context)
        {
            _context = context;
        }

        public void OnCompleted(Action continuation)
        {
            _context.Post(PostCallback, continuation);
        }

        public void GetResult()
        {
        }
    }
}