using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class AsyncEnumerableExtensions
    {
        // TODO: remove all calls to this
        public static ISeekableAsyncEnumerable<T> ToNonSeekable<T>(this IAsyncEnumerable<T> data)
            => ToNonSeekable(data, null);

        public static ISeekableAsyncEnumerable<T> ToNonSeekable<T>(this IAsyncEnumerable<T> data, ulong length)
            => ToNonSeekable(data, (ulong?) length);

        public static ISeekableAsyncEnumerable<T> ToNonSeekable<T>(this IAsyncEnumerable<T> data, ulong? length)
            => new NonSeekableAsyncEnumerable<T>(data, length);

        public static IAsyncEnumerable<Memory<T>> GroupByLengthAsync<T>(this IAsyncEnumerable<T> values, int length, CancellationToken cancellationToken = default)
            => new AsyncEnumerable<Memory<T>>(c => values.GetAsyncEnumerator(c).GroupByLengthAsync(length, cancellationToken));

        public static Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> values, CancellationToken cancellationToken = default)
            => values?.GetAsyncEnumerator(cancellationToken).ToArrayAsync(cancellationToken) ?? throw new ArgumentNullException(nameof(values));

        public static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> values, CancellationToken cancellationToken = default)
            => values?.GetAsyncEnumerator(cancellationToken).ToListAsync(cancellationToken) ?? throw new ArgumentNullException(nameof(values));

        public static IAsyncEnumerable<TOutput> SelectManyAsync<T, TOutput>(this IAsyncEnumerable<T> values, Func<T, IEnumerable<TOutput>> selector, CancellationToken cancellationToken = default)
            => new AsyncEnumerable<TOutput>(c => values.GetAsyncEnumerator(c).SelectManyAsync(selector, cancellationToken));

        public static IAsyncEnumerable<TOutput> SelectAsync<T, TOutput>(this IAsyncEnumerable<T> values, Func<T, TOutput> selector, CancellationToken cancellationToken = default)
            => new AsyncEnumerable<TOutput>(c => values.GetAsyncEnumerator(c).SelectAsync(selector, cancellationToken));

        public static IAsyncEnumerable<T> ForEachAsync<T>(this IAsyncEnumerable<T> values, Action<int, T> visitor, CancellationToken cancellationToken = default)
            => new AsyncEnumerable<T>(c => values.GetAsyncEnumerator(c).ForEachAsync(visitor, cancellationToken));

        // TODO: make internal?
        /*   How to write your own seekable enumerable manipulator
         * 
         *   public static ISeekableAsyncEnumerable<Sample> SelectAsync(ISeekableAsyncEnumerable<Sample> value, CancellationToken cancellationToken)
         *   {
         *       _ = value ?? throw new ArgumentNullException(nameof(value));
         *
         *       var enumerator = value.GetAsyncEnumerator(cancellationToken);
         *       var enumerable = SelectInnerAsync(enumerator, cancellationToken);
         *       return enumerable.ToSeekable(enumerator, value.DisposeAsync, value.GetDataLength());
         *   }
         *
         *   private static async IAsyncEnumerable<Sample> SelectInnerAsync(ISeekableAsyncEnumerator<Sample> value, [EnumeratorCancellation] CancellationToken cancellationToken)
         *   {
         *       while (await value.MoveNextAsync().ConfigureAwait(false))
         *       {
         *           if (cancellationToken.IsCancellationRequested)
         *           {
         *               throw new OperationCanceledException();
         *           }
         *
         *           yield return value.Current;
         *       }
         *   }
         */
        public static ISeekableAsyncEnumerable<TOut> ToSeekable<TIn, TOut>(this IAsyncEnumerable<TOut> data, ISeekableAsyncEnumerator<TIn> source, Func<ValueTask> disposeAction, ulong? newLength)
            => new SeekableAsyncEnumerableWrapper<TIn, TOut>(source, disposeAction, data, newLength);
    }
}
