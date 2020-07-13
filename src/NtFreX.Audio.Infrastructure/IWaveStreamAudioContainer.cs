using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Infrastructure
{
    public interface IWaveStreamAudioContainer : IStreamAudioContainer
    {
        IFmtSubChunk FmtSubChunk { [return: NotNull] get; }

        [return: NotNull] IAsyncEnumerable<byte[]> GetAudioSamplesAsync([MaybeNull] CancellationToken cancellationToken = default);
    }
}
