﻿using NtFreX.Audio.Containers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class AudioSamplerPipe
    {
        private readonly List<AudioSampler> samplers = new List<AudioSampler>();

        public AudioSamplerPipe Add(Func<AudioSamplerFactory, AudioSampler> sampler)
        {
            _ = sampler ?? throw new ArgumentNullException(nameof(sampler));

            samplers.Add(sampler(AudioSamplerFactory.Instance));
            return this;
        }
        public Task<IntermediateEnumerableAudioContainer> RunAsync(IntermediateListAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            return RunAsync(audio.AsEnumerable(), cancellationToken);
        }

        public async Task<IntermediateEnumerableAudioContainer> RunAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            IntermediateEnumerableAudioContainer currentAudio = audio;
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
