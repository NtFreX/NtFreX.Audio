using System;
using System.Threading;

namespace NtFreX.Audio.Infrastructure.Threading
{
    // TODO: delete inheritance of IAsyncDisposable?
    public interface ISeekableAsyncEnumerable<T> : IAsyncDisposable
    {
        long GetDataLength();

        ISeekableAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default);
    }
}
