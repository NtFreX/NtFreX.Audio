using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal class SeekableAsyncEnumerableWrapper<TSource, TValue> : ISeekableAsyncEnumerable<TValue>
    {
        private readonly ISeekableAsyncEnumerator<TSource> source;
        private readonly Func<ValueTask> disposeAction;
        private readonly IAsyncEnumerable<TValue> data;
        private readonly long dataLength;

        // TODO: was this the poor mans choise? still nessesary to work with enumerator in samplers so multiple enumeration does work without reset?
        public SeekableAsyncEnumerableWrapper(ISeekableAsyncEnumerator<TSource> source, Func<ValueTask> disposeAction, IAsyncEnumerable<TValue> data, long dataLength)
        {
            this.source = source;
            this.disposeAction = disposeAction;
            this.data = data;
            this.dataLength = dataLength;
        }

        public long GetDataLength()
            => dataLength;

        public ValueTask DisposeAsync()
            => disposeAction();

        public ISeekableAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new SeekableAsyncEnumeratorWrapper<TSource, TValue>(
                source, 
                data.GetAsyncEnumerator(cancellationToken),
                dataLength);
    }
}
