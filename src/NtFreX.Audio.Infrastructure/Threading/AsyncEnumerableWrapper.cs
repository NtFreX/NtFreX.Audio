using System.Collections.Generic;
using System.Threading;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal sealed class AsyncEnumerableWrapper<T> : IAsyncEnumerable<T>
    {
        private readonly IEnumerable<T> enumerable;
        private readonly bool runSynchronously;

        public AsyncEnumerableWrapper(IEnumerable<T> enumerable, bool runSynchronously)
        {
            this.enumerable = enumerable;
            this.runSynchronously = runSynchronously;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new AsyncEnumeratorWrapper<T>(enumerable.GetEnumerator(), runSynchronously, cancellationToken);
    }
}
