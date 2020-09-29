using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal class SeekableArrayEnumerator<T> : ISeekableAsyncEnumerator<T>
    {
        private readonly T[] data;
        private readonly CancellationToken cancellationToken;
        private long position = -1;

        public T Current { get; private set; }

        public SeekableArrayEnumerator(T[] data, CancellationToken cancellationToken)
        {
            Current = default!;
            this.data = data;
            this.cancellationToken = cancellationToken;
        }

        public long GetDataLength()
            => data.Length;
        public bool CanSeek()
            => true;
        public void SeekTo(long position)
            => this.position = position;
        public long GetPosition()
            => position;

        public ValueTask<bool> MoveNextAsync()
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            if (++position >= GetDataLength())
            {
                Current = default!;
                return new ValueTask<bool>(Task.FromResult(false));
            }

            Current = data[position];
            return new ValueTask<bool>(Task.FromResult(true));
        }

        public ValueTask DisposeAsync()
            => new ValueTask(Task.CompletedTask);
    }
}
