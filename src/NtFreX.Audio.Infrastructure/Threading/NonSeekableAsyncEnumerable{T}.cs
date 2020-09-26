using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    public class NonSeekableAsyncEnumerable<T> : ISeekableAsyncEnumerable<T>
    {
        private readonly IAsyncEnumerable<T> value;
        private readonly long length;

        public NonSeekableAsyncEnumerable(IAsyncEnumerable<T> value, long length)
        {
            this.value = value;
            this.length = length;
        }

        public long GetDataLength()
            => length;
        public ValueTask DisposeAsync()
            => new ValueTask(Task.CompletedTask);

        public ISeekableAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new NonSeekableAsyncEnumerator<T>(value.GetAsyncEnumerator(cancellationToken), length);
    }
}
