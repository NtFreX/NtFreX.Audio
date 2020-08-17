﻿using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class BitsPerSampleAudioSampler : AudioSampler
    {
        private readonly ushort bitsPerSample;

        public BitsPerSampleAudioSampler(ushort bitsPerSample)
        {
            this.bitsPerSample = bitsPerSample;
        }

        [return:NotNull] public override Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if (audio.FmtSubChunk.BitsPerSample == bitsPerSample)
            {
                return Task.FromResult(audio);
            }

            //TODO: make this work correctly with and from all sample rates
            // HINT: doubling bits per sample and not changing data will double speed
            return Task.FromResult(audio
                .WithFmtSubChunk(x => x
                    .WithBitsPerSample(bitsPerSample))
                .WithDataSubChunk(x => x
                    .WithChunkSize(audio.DataSubChunk.ChunkSize / audio.FmtSubChunk.BitsPerSample * bitsPerSample)
                    .WithData(SampleInnerAsync(audio, cancellationToken))));
        }

        public override string ToString()
        {
            return base.ToString() + $", bitsPerSample={bitsPerSample}";
        }

        private async IAsyncEnumerable<Sample> SampleInnerAsync(WaveEnumerableAudioContainer audio, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var isNewBigger = bitsPerSample > audio.FmtSubChunk.BitsPerSample;
            var factor = System.Math.Pow(256, isNewBigger ? bitsPerSample / audio.FmtSubChunk.BitsPerSample : audio.FmtSubChunk.BitsPerSample / bitsPerSample);
            var definition = new SampleDefinition(audio.Format.Type, bitsPerSample, audio.IsDataLittleEndian());
            await foreach (var sample in audio.GetAudioSamplesAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return new Sample(
                    audio.Format.Type switch
                    {
                        AudioFormatType.Pcm => isNewBigger ? sample.Value * factor : sample.Value / factor,
                        AudioFormatType.IeeFloat => sample.Value,
                        _ => throw new Exception()
                    }, definition);
            }
        }
    }
}
