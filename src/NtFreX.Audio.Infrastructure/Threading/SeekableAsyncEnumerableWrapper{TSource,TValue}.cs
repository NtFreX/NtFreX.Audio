using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal class SeekableAsyncEnumerableWrapper<TSource, TValue> : ISeekableAsyncEnumerable<TValue>
    {
        private readonly ISeekableAsyncEnumerable<TSource> source;
        private readonly IAsyncEnumerable<TValue> data;
        private readonly long dataLength;

        public SeekableAsyncEnumerableWrapper(ISeekableAsyncEnumerable<TSource> source, IAsyncEnumerable<TValue> data, long dataLength)
        {
            this.source = source;
            this.data = data;
            this.dataLength = dataLength;
        }

        public long GetDataLength()
            => dataLength;
        public ValueTask DisposeAsync()
            => source.DisposeAsync();
        
        public ISeekableAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new SeekableAsyncEnumeratorWrapper<TSource, TValue>(
                source.GetAsyncEnumerator(cancellationToken), 
                data.GetAsyncEnumerator(cancellationToken),
                dataLength);
    }
}
