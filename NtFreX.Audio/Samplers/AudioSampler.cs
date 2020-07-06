using NtFreX.Audio.Containers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public abstract class AudioSampler
    {
        [return: NotNull]
        protected static Action<double> AsRelativeProgress([NotNull] Action<double> progress, double max)
            => p => progress(p / max);

        [return: NotNull]
        public Task<WaveAudioContainer> SampleAsync([NotNull] WaveAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
            => SampleAsync(audio, x => { }, cancellationToken);

        [return: NotNull]
        public abstract Task<WaveAudioContainer> SampleAsync([NotNull] WaveAudioContainer audio, [NotNull] Action<double> onProgress, [MaybeNull] CancellationToken cancellationToken = default);
    }
}
