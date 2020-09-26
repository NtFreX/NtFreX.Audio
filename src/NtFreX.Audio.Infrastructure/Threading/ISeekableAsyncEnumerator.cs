using System;
using System.Collections.Generic;

namespace NtFreX.Audio.Infrastructure.Threading
{
    public interface ISeekableAsyncEnumerator<T> : IAsyncEnumerator<T>, IAsyncDisposable
    {
        long GetDataLength();

        bool CanSeek();
        void SeekTo(long position);
    }
}
