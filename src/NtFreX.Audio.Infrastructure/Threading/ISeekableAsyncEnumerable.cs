using System;
using System.Threading;

namespace NtFreX.Audio.Infrastructure.Threading
{
    public interface ISeekableAsyncEnumerable<T> : IAsyncDisposable
    {
        long GetDataLength();

        ISeekableAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default);
    }
}
