using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Infrastructure.Container
{
    public interface IWaveAudioContainer : IAudioContainer, IRiffContainer
    {
        IAudioFormat Format { get; }

        IAsyncEnumerable<Sample> GetAudioSamplesAsync([MaybeNull] CancellationToken cancellationToken = default);
    }
}
