using System.Collections.Generic;
using System.Threading;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal sealed class AsyncEnumerableWrapper<T> : IAsyncEnumerable<T>
    {
        private readonly IEnumerable<T> enumerable;

        public AsyncEnumerableWrapper(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new AsyncEnumeratorWrapper<T>(enumerable.GetEnumerator(), cancellationToken);
    }
}
