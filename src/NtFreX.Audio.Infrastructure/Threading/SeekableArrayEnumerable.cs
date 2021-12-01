using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal class SeekableArrayEnumerable<T> : ISeekableAsyncEnumerable<T>
    {
        private readonly T[] data;

        public SeekableArrayEnumerable(T[] data)
        {
            this.data = data;
        }

        public ulong GetDataLength()
            => (ulong) data.Length;
        public bool CanGetLength()
            => true;

        public ISeekableAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new SeekableArrayEnumerator<T>(data, cancellationToken);

        public ValueTask DisposeAsync()
            => new ValueTask(Task.CompletedTask);
    }
}
