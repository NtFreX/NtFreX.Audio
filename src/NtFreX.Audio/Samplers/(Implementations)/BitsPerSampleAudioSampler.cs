using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Math;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
            var isNewBigger = bitsPerSample > audio.FmtSubChunk.BitsPerSample;
            var factor = System.Math.Pow(16, isNewBigger ? bitsPerSample / audio.FmtSubChunk.BitsPerSample : audio.FmtSubChunk.BitsPerSample / bitsPerSample);
            var samples = audio.DataSubChunk.Data.SelectAsync(x => ((long)(isNewBigger ? x.ToInt64() * factor : x.ToInt64() / factor)).ToByteArray(bitsPerSample / 8));

            return Task.FromResult(audio
                .WithFmtSubChunk(x => x
                    .WithBitsPerSample(bitsPerSample))
                .WithDataSubChunk(x => x
                    .WithSubchunk2Size(audio.DataSubChunk.Subchunk2Size / audio.FmtSubChunk.BitsPerSample * bitsPerSample)
                    .WithData(samples)));
        }

        public override string ToString()
        {
            return base.ToString() + $", bitsPerSample={bitsPerSample}";
        }
    }
}
