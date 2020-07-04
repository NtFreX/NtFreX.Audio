using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NtFreX.Audio.Extensions
{
    internal static class AsyncEnumeratorExtensions
    {
        public static async IAsyncEnumerable<T[]> TakeAsync<T>(this IAsyncEnumerator<T[]> values, int length, [EnumeratorCancellation] CancellationToken cancellationToken = default)
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
