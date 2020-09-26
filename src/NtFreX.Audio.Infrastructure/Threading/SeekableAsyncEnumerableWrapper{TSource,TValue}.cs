using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal class SeekableAsyncEnumerableWrapper<TSource, TValue> : ISeekableAsyncEnumerable<TValue>
    {
        private readonly ISeekableAsyncEnumerable<TSource> source;
        private readonly IAsyncEnumerable<TValue> data;

        public SeekableAsyncEnumerableWrapper(ISeekableAsyncEnumerable<TSource> source, IAsyncEnumerable<TValue> data)
        {
            this.source = source;
            this.data = data;
        }

        public long GetDataLength()
            => source.GetDataLength();
        public ValueTask DisposeAsync()
            => source.DisposeAsync();
        
        public ISeekableAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new SeekableAsyncEnumeratorWrapper<TSource, TValue>(
                source.GetAsyncEnumerator(cancellationToken), 
                data.GetAsyncEnumerator(cancellationToken));
    }
}
