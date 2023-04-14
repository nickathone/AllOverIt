using AllOverIt.Assertion;
using System.Threading;

namespace AllOverIt.Wpf.Threading.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="SynchronizationContext"/>.</summary>
    public static class SynchronizationContextExtensions
    {
        /// <summary>Gets an awaiter for a synchronization context
        /// 
        /// </summary>
        /// <param name="synchronizationContext"></param>
        /// <returns></returns>
        public static SynchronizationContextAwaiter GetAwaiter(this SynchronizationContext synchronizationContext)
        {
            _ = synchronizationContext.WhenNotNull(nameof(synchronizationContext));

            return new SynchronizationContextAwaiter(synchronizationContext);
        }
    }
}