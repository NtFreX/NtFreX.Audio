using System;
using System.Threading;

namespace NtFreX.Audio.Infrastructure.Threading
{
    // TODO: delete inheritance of IAsyncDisposable?
    public interface ISeekableAsyncEnumerable<T> : IAsyncDisposable
    {
        ulong GetDataLength();
        bool CanGetLength();

        ISeekableAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default);
    }
}
