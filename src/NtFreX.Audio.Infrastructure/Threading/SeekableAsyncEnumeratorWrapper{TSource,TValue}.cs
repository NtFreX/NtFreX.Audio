using System.Collections.Generic;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal class SeekableAsyncEnumeratorWrapper<TSource, TValue> : ISeekableAsyncEnumerator<TValue>
    {
        private readonly ISeekableAsyncEnumerator<TSource> source;
        private readonly IAsyncEnumerator<TValue> value;
        private readonly long dataLength;
        private long position = -1;

        public TValue Current => value.Current;

        public SeekableAsyncEnumeratorWrapper(ISeekableAsyncEnumerator<TSource> source, IAsyncEnumerator<TValue> value, long dataLength)
        {
            this.source = source;
            this.value = value;
            this.dataLength = dataLength;
        }

        public long GetDataLength()
            => dataLength;
        public bool CanSeek()
            => source.CanSeek();
        public void SeekTo(long position)
            => source.SeekTo((long) (source.GetDataLength() * (1.0f * position / GetDataLength())));
        public long GetPosition()
            => position;
        public ValueTask<bool> MoveNextAsync()
        {
            position++;
            return value.MoveNextAsync();
        }
        public ValueTask DisposeAsync()
            => source.DisposeAsync();
    }
}
