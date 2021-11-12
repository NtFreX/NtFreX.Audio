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
        private readonly ulong? dataLength;

        // TODO: was this the poor mans choise? still nessesary to work with enumerator in samplers so multiple enumeration does work without reset?
        public SeekableAsyncEnumerableWrapper(ISeekableAsyncEnumerator<TSource> source, Func<ValueTask> disposeAction, IAsyncEnumerable<TValue> data, ulong? dataLength)
        {
            this.source = source;
            this.disposeAction = disposeAction;
            this.data = data;
            this.dataLength = dataLength;
        }

        public ulong GetDataLength()
            => dataLength ?? throw new NotSupportedException();

        public bool CanGetLength()
            => dataLength != null;

        public ValueTask DisposeAsync()
            => disposeAction();

        public ISeekableAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new SeekableAsyncEnumeratorWrapper<TSource, TValue>(
                source, 
                data.GetAsyncEnumerator(cancellationToken),
                dataLength);
    }
}
