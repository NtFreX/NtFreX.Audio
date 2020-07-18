using NtFreX.Audio.Containers;
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
            // if factor > 2 then divide by 2 and make multiple passes

            return Task.FromResult(audio
                .WithFmtSubChunk(x => x.WithSampleRate(sampleRate))
                .WithDataSubChunk(x => x
                    .WithSubchunk2Size((uint) (factor * audio.DataSubChunk.Subchunk2Size))
                    .WithData(SampleInnerAsync(audio, cancellationToken))));
        }

        [return: NotNull]
        private async IAsyncEnumerable<byte[]> SampleInnerAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull][EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var reverseFactor = System.Math.Round(audio.FmtSubChunk.SampleRate / (double)sampleRate, 6);
            reverseFactor = (reverseFactor * 100000) % 2 != 0 ? reverseFactor - 0.00001 : reverseFactor;

            var factor = sampleRate / (float) audio.FmtSubChunk.SampleRate;
            var previous = 0L;
            var counter = 1d;
            await foreach (var value in audio.DataSubChunk.Data.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                var number = value.ToInt64();

                var leftOverDown = counter % reverseFactor;
                if (factor > 1 || (factor < 1 && leftOverDown == 0d))
                {
                    yield return value;
                }

                var leftOver = counter % reverseFactor;
                if (factor > 1 && leftOver == 0d)
                {
                    yield return ((number + previous) / 2).ToByteArray(audio.FmtSubChunk.BitsPerSample / 8);
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
