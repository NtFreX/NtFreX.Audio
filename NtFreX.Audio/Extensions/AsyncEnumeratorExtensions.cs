using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NtFreX.Audio.Extensions
{
    internal static class AsyncEnumeratorExtensions
    {
        [return: NotNull]
        public static async IAsyncEnumerable<T[]> TakeAsync<T>([NotNull] this IAsyncEnumerator<T[]> values, int length, [MaybeNull] [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            int counter = 0;
            while (await values.MoveNextAsync()/*.ConfigureAwait(false)*/)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException("A cancelation has been requested");
                }

                if (counter++ >= length)
                {
                    break;
                }

                yield return values.Current;
            }

            if (counter != length)
            {
                throw new InvalidOperationException("Enumeration doesn't contain enough items");
            }
        }
    }
}
