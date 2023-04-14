using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace AllOverIt.Wpf.Threading
{
    public readonly struct BindToDispatcherAwaiter : INotifyCompletion
    {
        private readonly Dispatcher _dispatcher;

        public bool IsCompleted => _dispatcher.CheckAccess();

        internal BindToDispatcherAwaiter(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void OnCompleted(Action continuation)
        {
            _dispatcher.BeginInvoke(continuation);
        }

        public void GetResult()
        {
        }
    }
}