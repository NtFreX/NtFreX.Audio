using System.Collections.Generic;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal class SeekableAsyncEnumeratorWrapper<TSource, TValue> : ISeekableAsyncEnumerator<TValue>
    {
        private readonly ISeekableAsyncEnumerator<TSource> source;
        private readonly IAsyncEnumerator<TValue> value;
        
        public TValue Current => value.Current;

        public SeekableAsyncEnumeratorWrapper(ISeekableAsyncEnumerator<TSource> source, IAsyncEnumerator<TValue> value)
        {
            this.source = source;
            this.value = value;
        }

        public long GetDataLength()
            => source.GetDataLength();
        public bool CanSeek()
            => source.CanSeek();
        public void SeekTo(long position)
            => source.SeekTo(position);
        public long GetPosition()
            => source.GetPosition();
        public ValueTask<bool> MoveNextAsync()
            => value.MoveNextAsync();
        public ValueTask DisposeAsync()
            => source.DisposeAsync();
    }
}
