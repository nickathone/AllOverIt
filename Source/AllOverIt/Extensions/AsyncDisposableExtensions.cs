using AllOverIt.Assertion;
using AllOverIt.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IAsyncDisposable"/>.</summary>
    public static class AsyncDisposableExtensions
    {
        /// <summary>Asynchronously disposes a collection of <see cref="IAsyncDisposable"/> instances.</summary>
        /// <param name="disposables">The collection of disposables to asynchronously dispose of.</param>
        /// <returns>A <see cref="ValueTask"/> that completes when all disposables have been disposed of.</returns>
        public static ValueTask DisposeAllAsync(this IEnumerable<IAsyncDisposable> disposables)
        {
            _ = disposables.WhenNotNull(nameof(disposables));

            var disposableConnections = new CompositeAsyncDisposable(disposables.ToArray());
            
            return disposableConnections.DisposeAsync();
        }
    }
}