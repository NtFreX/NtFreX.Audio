using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class AsyncEnumerableExtensions
    {
        public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> values, CancellationToken cancellationToken = default) => (await ToListAsync(values, cancellationToken)/*.ConfigureAwait(false)*/).ToArray();

        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> values, CancellationToken cancellationToken = default)
        {
            var list = new List<T>();
            await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                list.Add(value);
            }
            return list;
        }
    }
}
