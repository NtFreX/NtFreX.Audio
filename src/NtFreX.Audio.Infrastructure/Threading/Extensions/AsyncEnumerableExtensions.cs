using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class AsyncEnumerableExtensions
    {
        // TODO: remove all calls to this
        public static ISeekableAsyncEnumerable<T> ToNonSeekable<T>(this IAsyncEnumerable<T> data, long length)
            => new NonSeekableAsyncEnumerable<T>(data, length);

        public static async IAsyncEnumerable<byte[]> GroupByLength(this IAsyncEnumerable<byte> data, int length)
        {
            var buffer = new byte[length];
            var index = 0;
            await foreach (var value in data.ConfigureAwait(false))
            {
                buffer[index++] = value;
                if (index == length)
                {
                    yield return buffer;
                    buffer = new byte[length];
                    index = 0;
                }
            }
        }

        public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> values, CancellationToken cancellationToken = default) 
            => (await values.ToListAsync(cancellationToken).ConfigureAwait(false)).ToArray();

        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> values, CancellationToken cancellationToken = default)
        {
            var list = new List<T>();
            await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                list.Add(value);
            }
            return list;
        }

        public static async IAsyncEnumerable<TOutput> SelectManyAsync<T, TOutput>(this IAsyncEnumerable<T> values, Func<T, IEnumerable<TOutput>> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));

            await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                foreach(var subValue in selector(value))
                {
                    yield return subValue;
                }
            }
        }

        public static async IAsyncEnumerable<TOutput> SelectAsync<T, TOutput>(this IAsyncEnumerable<T> values, Func<T, TOutput> selector, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));

            await foreach(var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return selector(value);
            }
        }

        public static async IAsyncEnumerable<T> ForEachAsync<T>(this IAsyncEnumerable<T> values, Action<int, T> visitor, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = visitor ?? throw new ArgumentNullException(nameof(visitor));

            var index = 0;
            await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                visitor(index++, value);
                yield return value;
            }
        }
    }
}
