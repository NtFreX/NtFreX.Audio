using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class SampleRateAudioSampler : AudioSampler
    {
        private readonly uint sampleRate;

        public SampleRateAudioSampler(uint sampleRate)
        {
            this.sampleRate = sampleRate;
        }

        [return:NotNull] public override Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if (audio.FmtSubChunk.SampleRate == sampleRate)
            {
                return Task.FromResult(audio);
            }

            var factor = sampleRate / (double) audio.FmtSubChunk.SampleRate;

            return Task.FromResult(audio
                .WithFmtSubChunk(x => x.WithSampleRate(sampleRate))
                .WithDataSubChunk(x => x
                    .WithSubchunk2Size((uint) (factor * audio.DataSubChunk.Subchunk2Size))
                    .WithData(SampleInnerAsync(audio, factor, cancellationToken))));
        }

        [return: NotNull]
        private static async IAsyncEnumerable<byte[]> SampleInnerAsync([NotNull] WaveEnumerableAudioContainer audio, double factor, [MaybeNull][EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var previous = 0L;
            var counter = 1;
            await foreach (var value in audio.DataSubChunk.Data.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                var number = value.ToInt64();
                if (factor > 1 && counter % factor == 0)
                {
                    yield return ((number + previous) / 2).ToByteArray(audio.FmtSubChunk.BitsPerSample / 8);
                }

                if (factor < 1 && counter % (1d / factor) != 0) 
                {
                    yield return value;
                }

                counter++;
                previous = number;
            }
        }

        public override string ToString()
        {
            return base.ToString() + $", sampleRate={sampleRate}";
        }
    }
}
