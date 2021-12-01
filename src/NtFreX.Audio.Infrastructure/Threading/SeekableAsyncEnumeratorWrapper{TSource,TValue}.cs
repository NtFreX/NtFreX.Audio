using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal class SeekableAsyncEnumeratorWrapper<TSource, TValue> : ISeekableAsyncEnumerator<TValue>
    {
        private readonly ISeekableAsyncEnumerator<TSource> source;
        private readonly IAsyncEnumerator<TValue> value;
        private readonly ulong? dataLength;
        private long position = -1;

        public TValue Current => value.Current;

        public SeekableAsyncEnumeratorWrapper(ISeekableAsyncEnumerator<TSource> source, IAsyncEnumerator<TValue> value, ulong? dataLength)
        {
            this.source = source;
            this.value = value;
            this.dataLength = dataLength;
        }

        public ulong GetDataLength()
            => dataLength ?? throw new NotSupportedException();

        public bool CanGetLength()
            => dataLength != null;

        public bool CanSeek()
            => source.CanSeek();

        public void SeekTo(long position)
        {
            var sourceDataLength = source.GetDataLength();
            var dataLength = GetDataLength();

            if(sourceDataLength == 0 || dataLength == 0)
            {
                throw new NotSupportedException("The given enumerable has no data length or the source has no data length");
            }

            source.SeekTo((long)(sourceDataLength! * (1.0f * position / dataLength!)));
            this.position = position;
        }

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
