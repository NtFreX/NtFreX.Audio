using System;
using System.Collections.Generic;
using System.Threading;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal class AsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly Func<CancellationToken, IAsyncEnumerator<T>> enumeratorAccessor;

        public AsyncEnumerable(Func<CancellationToken, IAsyncEnumerator<T>> enumeratorAccessor)
        {
            this.enumeratorAccessor = enumeratorAccessor;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => enumeratorAccessor(cancellationToken);
    }
}
