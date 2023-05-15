using AllOverIt.Patterns.ResourceInitialization;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Extensions
{
    /// <summary>Contains extension methods for use with <see cref="SemaphoreSlim"/> instances.</summary>
    public static class SemaphoreSlimExtensions
    {
        /// <summary>Asynchronously waits to enter the <see cref="SemaphoreSlim"/>, while observing a <see cref="CancellationToken"/>.
        /// The returned <see cref="IDisposable"/> ensures the semaphore is released when disposed.</summary>
        /// <param name="semaphoreSlim">The <see cref="SemaphoreSlim"/> to asychronously wait for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="IDisposable"/> that releases the semaphore when disposed.</returns>
        [ExcludeFromCodeCoverage]
        public static async Task<IDisposable> DisposableWaitAsync(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken)
        {
            await semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);

            return new Raii<SemaphoreSlim>(() => semaphoreSlim, semaphore => semaphore.Release());
        }
    }
}