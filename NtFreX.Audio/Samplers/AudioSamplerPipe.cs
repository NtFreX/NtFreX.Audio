using NtFreX.Audio.Containers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class AudioSamplerPipe
    {
        private readonly List<AudioSampler> samplers = new List<AudioSampler>();

        public AudioSamplerPipe Add([NotNull] Func<AudioSamplerFactory, AudioSampler> sampler)
        {
            samplers.Add(sampler(AudioSamplerFactory.Instance));
            return this;
        }

        [return: NotNull]
        public Task<WaveAudioContainer> RunAsync([NotNull] WaveAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
            => RunAsync(audio, x => { }, cancellationToken);

        [return: NotNull]
        public async Task<WaveAudioContainer> RunAsync([NotNull] WaveAudioContainer audio, [NotNull] Action<double> onProgress, [MaybeNull] CancellationToken cancellationToken = default)
        {
            WaveAudioContainer currentAudio = audio;
            var count = (double)samplers.Count;
            for (var i = 0; i < count; i++)
            {
                currentAudio = await samplers[i].SampleAsync(currentAudio, p => onProgress(p / count + 1.0 / count * i), cancellationToken).ConfigureAwait(false);
            }
            onProgress(1);
            return currentAudio;
        }
    }
}
