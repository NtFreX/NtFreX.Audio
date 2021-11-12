using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal class NonSeekableAsyncEnumerable<T> : ISeekableAsyncEnumerable<T>
    {
        private readonly IAsyncEnumerable<T> value;
        private readonly ulong? length;

        public NonSeekableAsyncEnumerable(IAsyncEnumerable<T> value, ulong? length = null)
        {
            this.value = value;
            this.length = length;
        }

        public ulong GetDataLength() 
            => length ?? throw new NotSupportedException();

        public bool CanGetLength()
            => length != null;

        public ValueTask DisposeAsync()
            => new ValueTask(Task.CompletedTask);

        public ISeekableAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new NonSeekableAsyncEnumerator<T>(value.GetAsyncEnumerator(cancellationToken), length);
    }
}
