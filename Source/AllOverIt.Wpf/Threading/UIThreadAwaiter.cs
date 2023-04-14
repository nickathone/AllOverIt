using AllOverIt.Assertion;
using System;
using System.Runtime.CompilerServices;

namespace AllOverIt.Wpf.Threading
{
    public sealed class UIThreadAwaiter : INotifyCompletion
    {
        private bool _isBound;

        /// <summary>Always returns false. The <see cref="OnCompleted(Action)"/> method will ensure
        /// the continuation is invoked, or queued to be invoked, on the UI thread.</summary>
        public bool IsCompleted { get; private set; }

        internal UIThreadAwaiter()
        {
        }

        /// <summary>Queues the <paramref name="continuation"/> on the UI thread.</summary>
        /// <param name="continuation">The continuation to be queued on the UI thread.</param>
        public void OnCompleted(Action continuation)
        {
            UIThread.ContinueOnUIThread(isBound =>
            {
                _isBound = isBound;
                IsCompleted = true;

                continuation.Invoke();
            });
        }

        /// <summary>Asserts the continuation was invoked on the UI thread.</summary>
        /// <exception cref="InvalidOperationException">When the continuation could not be invoked on the UI thread.</exception>
        public void GetResult()
        {
            Throw<InvalidOperationException>.When(!_isBound, "Unable to bind to the UI thread.");
        }
    }
}