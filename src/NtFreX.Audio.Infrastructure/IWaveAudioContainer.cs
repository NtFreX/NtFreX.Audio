using NtFreX.Audio.Infrastructure;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Infrastructure
{
    public interface IWaveAudioContainer : IAudioContainer
    {
        IAudioFormat Format { get; }

        IAsyncEnumerable<Sample> GetAudioSamplesAsync([MaybeNull] CancellationToken cancellationToken = default);
    }
}
