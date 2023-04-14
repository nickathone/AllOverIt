using System.Windows.Threading;

namespace AllOverIt.Wpf.Threading
{
    public readonly struct BindToDispatcherAwaitable
    {
        private readonly Dispatcher _dispatcher;

        internal BindToDispatcherAwaitable(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public BindToDispatcherAwaiter GetAwaiter()
        {
            return new BindToDispatcherAwaiter(_dispatcher);
        }
    }
}