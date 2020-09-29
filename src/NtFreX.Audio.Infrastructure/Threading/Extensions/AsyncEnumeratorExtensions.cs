using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class AsyncEnumeratorExtensions
    {
        public static async IAsyncEnumerator<T[]> GroupByLengthAsync<T>(this IAsyncEnumerator<T> value, int length, CancellationToken cancellationToken = default)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            var buffer = new T[length];
            var index = 0;
            while (await value.MoveNextAsync().ConfigureAwait(false))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                buffer[index++] = value.Current;
                if (index == length)
                {
                    yield return buffer;
                    buffer = new T[length];
                    index = 0;
                }
            }
        }

        public static async IAsyncEnumerator<T> ForEachAsync<T>(this IAsyncEnumerator<T> value, Action<int, T> action, CancellationToken cancellationToken = default)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));
            _ = action ?? throw new ArgumentNullException(nameof(action));

            var index = 0;
            while (await value.MoveNextAsync().ConfigureAwait(false))
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                action(index++, value.Current);
                yield return value.Current;
            }
        }

        public static async IAsyncEnumerator<TTarget> SelectAsync<TSource, TTarget>(this IAsyncEnumerator<TSource> value, Func<TSource, TTarget> selector, CancellationToken cancellationToken = default)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));

            while (await value.MoveNextAsync().ConfigureAwait(false))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                yield return selector(value.Current);
            }
        }

        public static async IAsyncEnumerator<TOutput> SelectManyAsync<T, TOutput>(this IAsyncEnumerator<T> value, Func<T, IEnumerable<TOutput>> selector, CancellationToken cancellationToken = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            _ = value ?? throw new ArgumentNullException(nameof(value));

            while (await value.MoveNextAsync().ConfigureAwait(false))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                foreach (var subValue in selector(value.Current))
                {
                    yield return subValue;
                }
            }
        }

        public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerator<T> value, CancellationToken cancellationToken = default)
            => (await value.ToListAsync(cancellationToken).ConfigureAwait(false)).ToArray();

        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerator<T> value, CancellationToken cancellationToken = default)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            var list = new List<T>();
            while (await value.MoveNextAsync().ConfigureAwait(false))
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                list.Add(value.Current);
            }
            return list;
        }

        internal static ISeekableAsyncEnumerator<TOut> ToSeekable<TIn, TOut>(this IAsyncEnumerator<TOut> data, ISeekableAsyncEnumerator<TIn> source, long newLength)
            => new SeekableAsyncEnumeratorWrapper<TIn, TOut>(source, data, newLength);
    }
}
