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
            var newDataSize = System.Math.Round(factor * audio.DataSubChunk.Subchunk2Size, 0);
            while(factor > 0)
            {
                audio = audio.WithDataSubChunk(x => x.WithData(SampleInnerAsync(audio, newDataSize, cancellationToken)));
                factor -= 2;
            }

            return Task.FromResult(audio
                .WithFmtSubChunk(x => x.WithSampleRate(sampleRate))
                .WithDataSubChunk(x => x.WithSubchunk2Size((uint)newDataSize)));
        }

        [return: NotNull]
        private async IAsyncEnumerable<byte[]> SampleInnerAsync([NotNull] WaveEnumerableAudioContainer audio, double newDataSize, [MaybeNull][EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var factor = sampleRate / (float)audio.FmtSubChunk.SampleRate;
            var sizeOfParts = audio.DataSubChunk.Subchunk2Size / (double) System.Math.Abs(audio.DataSubChunk.Subchunk2Size - newDataSize);
            var previous = 0L;
            var counter = 1d;
            var total = 0L;
            await foreach (var value in audio.DataSubChunk.Data.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                var number = value.ToInt64();

                var positionReached = counter > sizeOfParts;

                // upsampling
                if (factor > 1)
                {
                    if (positionReached)
                    {
                        yield return ((number + previous) / 2).ToByteArray(audio.FmtSubChunk.BitsPerSample / 8);
                        counter -= sizeOfParts;
                    }

                    yield return value;
                }

                // downsampling
                if (factor < 1)
                {
                    if (positionReached)
                    {
                        counter -= sizeOfParts;
                    }
                    else if(total < newDataSize)
                    {
                        yield return value;
                        total += audio.FmtSubChunk.BitsPerSample / 8;
                    }
                }

                counter++;
                previous = number;
            }

            if (factor > 1 && counter > sizeOfParts && factor <= 2)
            {
                yield return ((0 + previous) / 2).ToByteArray(audio.FmtSubChunk.BitsPerSample / 8);
            }
        }

        public override string ToString()
        {
            return base.ToString() + $", sampleRate={sampleRate}";
        }
    }
}
