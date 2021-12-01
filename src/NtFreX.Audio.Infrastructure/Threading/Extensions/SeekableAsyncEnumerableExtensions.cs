using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class SeekableAsyncEnumerableExtensions
    {
        public static ISeekableAsyncEnumerable<T[]> GroupByLengthAsync<T>(this ISeekableAsyncEnumerable<T> values, int length, CancellationToken cancellationToken = default)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));

            var enumerator = values.GetAsyncEnumerator(cancellationToken);
            return new AsyncEnumerable<T[]>(c => enumerator.GroupByLengthAsync(length, c).ToAsyncEnumerator())
                    .ToSeekable(enumerator, values.DisposeAsync, values.CanGetLength() ? values.GetDataLength() / (ulong?) length : null);
        }

        public static ISeekableAsyncEnumerable<TOut> SelectManyAsync<TIn, TOut>(this ISeekableAsyncEnumerable<TIn> values, Func<TIn, IEnumerable<TOut>> selector, ulong? newSize, CancellationToken cancellationToken = default)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));

            var enumerator = values.GetAsyncEnumerator(cancellationToken);
            return new AsyncEnumerable<TOut>(c => enumerator.ToAsyncEnumerator().SelectManyAsync(selector, c))
                    .ToSeekable(enumerator, values.DisposeAsync, newSize);
        }

        public static ISeekableAsyncEnumerable<TOut> SelectAsync<TIn, TOut>(this ISeekableAsyncEnumerable<TIn> values, Func<TIn, TOut> selector, CancellationToken cancellationToken = default)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));

            var enumerator = values.GetAsyncEnumerator(cancellationToken);
            return new AsyncEnumerable<TOut>(c => enumerator.ToAsyncEnumerator().SelectAsync(selector, c))
                    .ToSeekable(enumerator, values.DisposeAsync, values?.GetDataLength() ?? throw new ArgumentNullException(nameof(values)));
        }

        public static ISeekableAsyncEnumerable<T> ForEachAsync<T>(this ISeekableAsyncEnumerable<T> values, Action<int, T> visitor, CancellationToken cancellationToken = default)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));

            var enumerator = values.GetAsyncEnumerator(cancellationToken);
            return new AsyncEnumerable<T>(c => enumerator.ToAsyncEnumerator().ForEachAsync(visitor, c))
                    .ToSeekable(enumerator, values.DisposeAsync, values?.GetDataLength() ?? throw new ArgumentNullException(nameof(values)));
        }

        public static async Task<T[]> ToArrayAsync<T>(this ISeekableAsyncEnumerable<T> values, CancellationToken cancellationToken = default)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));

            await using var enumerator = values.GetAsyncEnumerator(cancellationToken);
            return await enumerator.ToArrayAsync(cancellationToken).ConfigureAwait(false);
        }

        public static async Task<List<T>> ToListAsync<T>(this ISeekableAsyncEnumerable<T> values, CancellationToken cancellationToken = default)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));

            await using var enumerator = values.GetAsyncEnumerator(cancellationToken);
            return await enumerator.ToListAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
