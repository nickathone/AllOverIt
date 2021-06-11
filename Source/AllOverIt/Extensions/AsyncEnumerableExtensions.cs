using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Extensions
{
#if !NETSTANDARD2_0

    public static class AsyncEnumerableExtensions
    {
        public static async Task<List<TType>> ToListAsync<TType>(this IAsyncEnumerable<TType> items,
            CancellationToken cancellationToken = default)
        {
            var listItems = new List<TType>();

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                listItems.Add(item);
            }

            return listItems;
        }
    }
#endif
}