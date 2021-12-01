using NtFreX.Audio.Infrastructure.Threading;
using System;
using System.Threading;

namespace NtFreX.Audio.Infrastructure.Container
{
    // TODO: why both IDisposable and IAsyncDisposable isn't one enough?
    public interface IAudioContainer : IDisposable, IAsyncDisposable
    {
        IAudioFormat GetFormat();
        TimeSpan GetLength();
        ulong GetByteLength();
        bool CanGetLength();
        bool IsDataLittleEndian();

        ISeekableAsyncEnumerable<Memory<byte>> GetAsyncAudioEnumerable(CancellationToken cancellationToken = default);
    }
}
