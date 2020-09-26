using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    public class SeekableAsyncEnumerable<T> : ISeekableAsyncEnumerable<T>
    {
        private readonly Func<CancellationToken, ISeekableAsyncEnumerator<T>> enumeratorAccessor;
        private readonly long length;

        public SeekableAsyncEnumerable(Func<CancellationToken, ISeekableAsyncEnumerator<T>> enumeratorAccessor, long length)
        {
            this.enumeratorAccessor = enumeratorAccessor;
            this.length = length;
        }

        public ValueTask DisposeAsync()
            => new ValueTask(Task.CompletedTask);

        public ISeekableAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => enumeratorAccessor(cancellationToken);

        public long GetDataLength()
            => length;
    }
}
