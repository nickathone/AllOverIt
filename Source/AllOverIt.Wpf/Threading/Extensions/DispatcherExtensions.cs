using AllOverIt.Assertion;
using System.Windows.Threading;

namespace AllOverIt.Wpf.Threading.Extensions
{
    public static class DispatcherExtensions
    {
        public static BindToDispatcherAwaitable BindTo(this Dispatcher dispatcher)
        {
            _ = dispatcher.WhenNotNull(nameof(dispatcher));

            return new BindToDispatcherAwaitable(dispatcher);
        }
    }
}