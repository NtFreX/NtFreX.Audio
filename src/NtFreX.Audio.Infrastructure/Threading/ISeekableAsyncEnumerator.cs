using System;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    public interface ISeekableAsyncEnumerator<T> : IAsyncDisposable
    {
        T Current { get; }

        long GetDataLength();
        long GetPosition();

        bool CanSeek();
        void SeekTo(long position);

        ValueTask<bool> MoveNextAsync();
    }
}
