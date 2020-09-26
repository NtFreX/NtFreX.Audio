using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class SeekableAsyncEnumerableExtensions
    {
        public static ISeekableAsyncEnumerable<T[]> GroupByLengthAsync<T>(this ISeekableAsyncEnumerable<T> values, int length, CancellationToken cancellationToken = default)
            => GroupByLengthInnerAsync(values, length, cancellationToken).ToSeekable(values, values.GetDataLength() / length);

        private static async IAsyncEnumerable<T[]> GroupByLengthInnerAsync<T>(this ISeekableAsyncEnumerable<T> values, int length, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // TODO: stop multiplicating enumerable extension code?
            var buffer = new T[length];
            var index = 0;
            await foreach (var value in values)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                buffer[index++] = value;
                if (index == length)
                {
                    yield return buffer;
                    buffer = new T[length];
                    index = 0;
                }
            }
        }

        public static ISeekableAsyncEnumerable<TOut> SelectManyAsync<TIn, TOut>(this ISeekableAsyncEnumerable<TIn> values, Func<TIn, IEnumerable<TOut>> selector, long newSize, CancellationToken cancellationToken = default)
            => SelectManyInnerAsync(values, selector, cancellationToken).ToSeekable(values, newSize);

        private static async IAsyncEnumerable<TOut> SelectManyInnerAsync<TIn, TOut>(this ISeekableAsyncEnumerable<TIn> values, Func<TIn, IEnumerable<TOut>> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            _ = values ?? throw new ArgumentNullException(nameof(values));

            await foreach (var value in values)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                foreach (var newValue in selector(value)) 
                {
                    yield return newValue;
                }
            }
        }

        public static ISeekableAsyncEnumerable<TOutput> SelectAsync<T, TOutput>(this ISeekableAsyncEnumerable<T> values, Func<T, TOutput> selector, CancellationToken cancellationToken = default)
            => SelectInnerAsync(values, selector, cancellationToken).ToSeekable(values, values.GetDataLength());

        private static async IAsyncEnumerable<TOutput> SelectInnerAsync<T, TOutput>(this ISeekableAsyncEnumerable<T> values, Func<T, TOutput> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            _ = values ?? throw new ArgumentNullException(nameof(values));

            await foreach (var value in values)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                yield return selector(value);
            }
        }

        public static ISeekableAsyncEnumerable<T> ForEachAsync<T>(this ISeekableAsyncEnumerable<T> values, Action<int, T> visitor, CancellationToken cancellationToken = default)
            => ForEachInnerAsync(values, visitor, cancellationToken).ToSeekable(values, values.GetDataLength());

        private static async IAsyncEnumerable<T> ForEachInnerAsync<T>(this ISeekableAsyncEnumerable<T> values, Action<int, T> visitor, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = values ?? throw new ArgumentNullException(nameof(values));
            _ = visitor ?? throw new ArgumentNullException(nameof(visitor));

            var index = 0;
            await foreach (var value in values)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

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
