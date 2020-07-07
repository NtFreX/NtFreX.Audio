using NtFreX.Audio.Containers;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public abstract class AudioSampler
    {
        [return: NotNull]
        public Task<WaveAudioContainerStream> SampleAsync([NotNull] WaveAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
            => SampleAsync(WaveAudioContainerStream.ToStream(audio, cancellationToken), cancellationToken);

        [return: NotNull]
        public abstract Task<WaveAudioContainerStream> SampleAsync([NotNull] WaveAudioContainerStream audio, [MaybeNull] CancellationToken cancellationToken = default);
    }
}
