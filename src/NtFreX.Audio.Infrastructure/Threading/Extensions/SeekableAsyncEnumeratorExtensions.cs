using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class SeekableAsyncEnumeratorExtensions
    { 
        public static ISeekableAsyncEnumerator<Memory<T>> GroupByLengthAsync<T>(this ISeekableAsyncEnumerator<T> values, int length, CancellationToken cancellationToken = default)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));

            return values.ToAsyncEnumerator()
                .GroupByLengthAsync(length, cancellationToken)
                .ToSeekable(values, values.CanGetLength() ? values.GetDataLength() / (ulong?) length : null);
        }

        public static ISeekableAsyncEnumerator<TOut> SelectManyAsync<TIn, TOut>(this ISeekableAsyncEnumerator<TIn> values, Func<TIn, IEnumerable<TOut>> selector, ulong? newSize, CancellationToken cancellationToken = default)
            => values?.ToAsyncEnumerator().SelectManyAsync(selector, cancellationToken).ToSeekable(values, newSize)
                ?? throw new ArgumentNullException(nameof(values));

        public static ISeekableAsyncEnumerator<TOut> SelectAsync<TIn, TOut>(this ISeekableAsyncEnumerator<TIn> values, Func<TIn, TOut> selector, CancellationToken cancellationToken = default)
             => values?.ToAsyncEnumerator().SelectAsync(selector, cancellationToken).ToSeekable(values, values?.GetDataLength() ?? throw new ArgumentNullException(nameof(values)))
                ?? throw new ArgumentNullException(nameof(values));

        public static ISeekableAsyncEnumerator<T> ForEachAsync<T>(this ISeekableAsyncEnumerator<T> values, Action<int, T> visitor, CancellationToken cancellationToken = default)
            => values?.ToAsyncEnumerator().ForEachAsync(visitor, cancellationToken).ToSeekable(values, values?.GetDataLength() ?? throw new ArgumentNullException(nameof(values)))
                ?? throw new ArgumentNullException(nameof(values));

        public static Task<T[]> ToArrayAsync<T>(this ISeekableAsyncEnumerator<T> values, CancellationToken cancellationToken = default)
            => values?.ToAsyncEnumerator().ToArrayAsync(cancellationToken) ?? throw new ArgumentNullException(nameof(values));

        public static Task<List<T>> ToListAsync<T>(this ISeekableAsyncEnumerator<T> values, CancellationToken cancellationToken = default)
            => values?.ToAsyncEnumerator().ToListAsync(cancellationToken) ?? throw new ArgumentNullException(nameof(values));

        internal static IAsyncEnumerator<T> ToAsyncEnumerator<T>(this ISeekableAsyncEnumerator<T> values)
            => new AsyncEnumerator<T>(values.MoveNextAsync, () => values.Current, values.DisposeAsync);
    }
}
