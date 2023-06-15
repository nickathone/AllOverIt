using AllOverIt.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllOverIt.Extensions
{
    public static class DisposableExtensions
    {
        public static ValueTask DisposeAllAsync(this IEnumerable<IAsyncDisposable> diposables)
        {
            var disposableConnections = new CompositeAsyncDisposable(diposables.ToArray());
            
            return disposableConnections.DisposeAsync();
        }
    }
}