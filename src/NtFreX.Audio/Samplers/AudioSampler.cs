using NtFreX.Audio.Containers;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public abstract class AudioSampler
    {
        [return: NotNull]
        public Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveStreamAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
            => SampleAsync(WaveEnumerableAudioContainer.ToEnumerable(audio, cancellationToken), cancellationToken);

        [return: NotNull]
        public abstract Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default);
    }
}
