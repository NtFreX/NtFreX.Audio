using NtFreX.Audio.Infrastructure.Threading;
using System;
using System.Collections.Generic;
using System.Threading;

namespace NtFreX.Audio.Infrastructure.Container
{
    // TODO: why both IDisposable and IAsyncDisposable isn't one enough?
    public interface IAudioContainer : IDisposable, IAsyncDisposable
    {
        IAudioFormat GetFormat();
        TimeSpan GetLength();
        long GetByteLength();
        bool IsDataLittleEndian();

        ISeekableAsyncEnumerable<IReadOnlyList<byte>> GetAsyncAudioEnumerable(CancellationToken cancellationToken = default);
    }
}
