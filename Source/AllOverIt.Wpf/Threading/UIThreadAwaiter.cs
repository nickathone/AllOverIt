using AllOverIt.Assertion;
using System;
using System.Runtime.CompilerServices;

namespace AllOverIt.Wpf.Threading
{
    public sealed class UIThreadAwaiter : INotifyCompletion
    {
        private bool _isBound;

        internal UIThreadAwaiter()
        {
        }

        // UIThread.ContinueOnUIThread() will determine if we're already on the UI thread or not
        public bool IsCompleted { get; private set; }

        public void OnCompleted(Action continuation)
        {
            UIThread.ContinueOnUIThread(isBound =>
            {
                _isBound = isBound;
                IsCompleted = true;

                continuation.Invoke();
            });
        }

        public void GetResult()
        {
            Throw<InvalidOperationException>.When(!_isBound, "Unable to bind to the UI thread.");
        }
    }
}