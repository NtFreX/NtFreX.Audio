﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    public static class AsyncEnumerableExtensions
    {
        [return:NotNull] public static async Task<T[]> ToArrayAsync<T>([NotNull] this IAsyncEnumerable<T> values, [MaybeNull] CancellationToken cancellationToken = default) 
            => (await values.ToListAsync(cancellationToken).ConfigureAwait(false)).ToArray();

        [return: NotNull] public static async Task<List<T>> ToListAsync<T>([NotNull] this IAsyncEnumerable<T> values, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var list = new List<T>();
            await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                list.Add(value);
            }
            return list;
        }

        [return: NotNull] public static async IAsyncEnumerable<TOutput> SelectManyAsync<T, TOutput>([NotNull] this IAsyncEnumerable<T> values, [NotNull] Func<T, IEnumerable<TOutput>> selector, [MaybeNull][EnumeratorCancellation] CancellationToken cancellationToken = default)
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

        [return: NotNull] public static async IAsyncEnumerable<TOutput> SelectAsync<T, TOutput>([NotNull] this IAsyncEnumerable<T> values, [NotNull] Func<T, TOutput> selector, [MaybeNull] [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));

            await foreach(var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return selector(value);
            }
        }

        [return: NotNull]
        public static async IAsyncEnumerable<T> ForEachAsync<T>([NotNull] this IAsyncEnumerable<T> values, [NotNull] Action<int, T> visitor, [MaybeNull][EnumeratorCancellation] CancellationToken cancellationToken = default)
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
