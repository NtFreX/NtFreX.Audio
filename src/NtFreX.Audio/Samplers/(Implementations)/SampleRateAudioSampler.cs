using NtFreX.Audio.Containers;
using System;
using System.Diagnostics.CodeAnalysis;
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
            var originFactor = factor;
            var newDataSize = System.Math.Round(factor * audio.DataSubChunk.ChunkSize, 0);
            while(factor > 0)
            {
                audio = audio.WithDataSubChunk(x => x.WithData(WaveStretcher.StretchAsync(audio, originFactor, cancellationToken)));
                factor -= 2;
            }

            return Task.FromResult(audio
                .WithFmtSubChunk(x => x.WithSampleRate(sampleRate))
                .WithDataSubChunk(x => x.WithChunkSize((uint)newDataSize)));
        }

        public override string ToString()
        {
            return base.ToString() + $", sampleRate={sampleRate}";
        }
    }
}
