using System.Collections.Generic;

namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class EnumerableExtensions
    {
        public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> enumerable)
            => new AsyncEnumerableWrapper<T>(enumerable);
    }
}
