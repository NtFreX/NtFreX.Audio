﻿using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class FromMonoAudioSampler : AudioSampler
    {
        private readonly int targetChannels;

        public FromMonoAudioSampler(int targetChannels)
        {
            this.targetChannels = targetChannels;
        }

        public override Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var format = audio.GetFormat();
            if(format.Channels != 1)
            {
                throw new ArgumentException("Only mono is supported");
            }

            if (format.Channels == targetChannels)
            {
                return Task.FromResult(audio);
            }

            var newSize = audio.CanGetLength() ? (ulong?) (audio.GetDataLength() * (ulong)targetChannels) : null;

            return Task.FromResult(audio.WithData(
                data: audio.SelectManyAsync(FromMono, newSize, cancellationToken),
                format: new AudioFormat(format.SampleRate, format.BitsPerSample, (ushort) targetChannels, format.Type)));
        }

        private IEnumerable<Sample> FromMono(Sample sample)
        {
            for (var i = 0; i < targetChannels; i++)
            {
                yield return sample;
            }
        }
    }
}
