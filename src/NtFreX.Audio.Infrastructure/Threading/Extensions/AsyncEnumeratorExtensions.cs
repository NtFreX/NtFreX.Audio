using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class AsyncEnumeratorExtensions
    {
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
    }
}
