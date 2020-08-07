using System.Collections.Generic;

namespace NtFreX.Audio.Infrastructure.Threading
{
    public static class EnumerableExtensions
    {
        public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable, bool runSynchronously = true)
            => new AsyncEnumerableWrapper<T>(enumerable, runSynchronously);
    }
}
