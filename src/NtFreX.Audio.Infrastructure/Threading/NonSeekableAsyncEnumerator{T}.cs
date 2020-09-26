using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal class NonSeekableAsyncEnumerator<T> : ISeekableAsyncEnumerator<T>
    {
        private readonly IAsyncEnumerator<T> value;
        private readonly long length;

        public T Current => value.Current;

        public NonSeekableAsyncEnumerator(IAsyncEnumerator<T> value, long length)
        {
            this.value = value;
            this.length = length;
        }

        public bool CanSeek()
            => false;
        public void SeekTo(long position)
            => throw new NotSupportedException();
        public long GetDataLength()
            => length;
        public ValueTask<bool> MoveNextAsync()
            => value.MoveNextAsync();
        public ValueTask DisposeAsync()
            => value.DisposeAsync();
    }
}
