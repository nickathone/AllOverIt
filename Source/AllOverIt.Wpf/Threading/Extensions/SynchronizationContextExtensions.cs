using AllOverIt.Assertion;
using System.Threading;

namespace AllOverIt.Wpf.Threading.Extensions
{
    public static class SynchronizationContextExtensions
    {
        public static SynchronizationContextAwaiter GetAwaiter(this SynchronizationContext synchronizationContext)
        {
            _ = synchronizationContext.WhenNotNull(nameof(synchronizationContext));

            return new SynchronizationContextAwaiter(synchronizationContext);
        }
    }
}