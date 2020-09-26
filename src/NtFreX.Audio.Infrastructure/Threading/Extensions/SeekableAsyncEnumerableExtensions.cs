using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class SeekableAsyncEnumerableExtensions
    {
        public static ISeekableAsyncEnumerable<TOutput> SelectAsync<T, TOutput>(this ISeekableAsyncEnumerable<T> values, Func<T, TOutput> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
            => new SeekableAsyncEnumerableWrapper<T, TOutput>(values, SelectInnerAsync(values, selector, cancellationToken));

        private static async IAsyncEnumerable<TOutput> SelectInnerAsync<T, TOutput>(this ISeekableAsyncEnumerable<T> values, Func<T, TOutput> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            _ = values ?? throw new ArgumentNullException(nameof(values));

            await foreach (var value in values)
            {
                yield return selector(value);
            }
        }

        public static ISeekableAsyncEnumerable<T> ForEachAsync<T>(this ISeekableAsyncEnumerable<T> values, Action<int, T> visitor, [EnumeratorCancellation] CancellationToken cancellationToken = default)
            => new SeekableAsyncEnumerableWrapper<T, T>(values, ForEachInnerAsync(values, visitor, cancellationToken));

        private static async IAsyncEnumerable<T> ForEachInnerAsync<T>(this ISeekableAsyncEnumerable<T> values, Action<int, T> visitor, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));
            _ = visitor ?? throw new ArgumentNullException(nameof(visitor));

            var index = 0;
            await foreach (var value in values)
            {
                visitor(index++, value);
                yield return value;
            }
        }

        public static async Task<T[]> ToArrayAsync<T>(this ISeekableAsyncEnumerable<T> values, CancellationToken cancellationToken = default)
            => (await values.ToListAsync(cancellationToken).ConfigureAwait(false)).ToArray();

        public static async Task<List<T>> ToListAsync<T>(this ISeekableAsyncEnumerable<T> values, CancellationToken cancellationToken = default)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));

            var list = new List<T>();
            await foreach (var value in values)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                list.Add(value);
            }
            return list;
        }
    }
}
