using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal class NonSeekableAsyncEnumerator<T> : ISeekableAsyncEnumerator<T>
    {
        private readonly IAsyncEnumerator<T> value;
        private readonly long? length;
        private long position = -1;

        public T Current => value.Current;

        public NonSeekableAsyncEnumerator(IAsyncEnumerator<T> value, long? length)
        {
            this.value = value;
            this.length = length;
        }

        public bool CanSeek()
            => false;
        public void SeekTo(long position)
            => throw new NotSupportedException();
        public long GetDataLength()
            => length ?? throw new NotSupportedException();
        public long GetPosition()
            => position;
        public ValueTask<bool> MoveNextAsync()
        {
            position++;
            return value.MoveNextAsync();
        }
        public ValueTask DisposeAsync()
            => value.DisposeAsync();
    }
}
