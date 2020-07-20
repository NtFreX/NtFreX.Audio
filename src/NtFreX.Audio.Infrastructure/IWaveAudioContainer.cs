using NtFreX.Audio.Infrastructure;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Infrastructure
{
    public interface IWaveAudioContainer : IAudioContainer
    {
        IFmtSubChunk FmtSubChunk { [return: NotNull] get; }

        IAsyncEnumerable<byte[]> GetAudioSamplesAsync([MaybeNull] CancellationToken cancellationToken = default);
    }
}
