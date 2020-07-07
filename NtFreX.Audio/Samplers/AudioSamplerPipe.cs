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
        public async Task<WaveAudioContainerStream> RunAsync([NotNull] WaveAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            WaveAudioContainerStream currentAudio = WaveAudioContainerStream.ToStream(audio, cancellationToken);
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
