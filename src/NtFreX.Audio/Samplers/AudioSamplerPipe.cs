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
            _ = sampler ?? throw new ArgumentNullException(nameof(sampler));

            samplers.Add(sampler(AudioSamplerFactory.Instance));
            return this;
        }

        [return: NotNull]
        public async Task<WaveEnumerableAudioContainer> RunAsync([NotNull] WaveStreamAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            WaveEnumerableAudioContainer currentAudio = WaveEnumerableAudioContainer.ToEnumerable(audio, cancellationToken);
            var count = (double)samplers.Count;
            for (var i = 0; i < count; i++)
            {
                var sampler = samplers[i];
                currentAudio = await sampler.SampleAsync(currentAudio, cancellationToken).ConfigureAwait(false);
            }            
            return currentAudio;
        }
    }
}
