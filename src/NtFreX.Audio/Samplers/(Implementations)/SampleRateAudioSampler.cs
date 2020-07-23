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

        [return:NotNull] public override async Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if (audio.FmtSubChunk.SampleRate == sampleRate)
            {
                return audio;
            }

            var pipe = new AudioSamplerPipe();
            var currentSampleRate = audio.FmtSubChunk.SampleRate;
            if (sampleRate > currentSampleRate)
            {
                while (currentSampleRate * 2 < sampleRate)
                {
                    pipe.Add(x => x.SampleRateAudioSampler(currentSampleRate * 2));
                    currentSampleRate *= 2;
                }
            }
            else
            {
                while (currentSampleRate / 2 > sampleRate)
                {
                    pipe.Add(x => x.SampleRateAudioSampler(currentSampleRate / 2));
                    currentSampleRate /= 2;
                }
            }

            var factor = sampleRate / (double)currentSampleRate;
            var preparedAudio = await pipe.RunAsync(audio, cancellationToken).ConfigureAwait(false);
            return SampleInner(preparedAudio, factor, cancellationToken);
        }

        public override string ToString()
        {
            return base.ToString() + $", sampleRate={sampleRate}";
        }

        private WaveEnumerableAudioContainer SampleInner([NotNull] WaveEnumerableAudioContainer audio, double factor, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var newDataSize = System.Math.Round(factor * audio.DataSubChunk.ChunkSize, 0);
            return audio
                .WithFmtSubChunk(x => x.WithSampleRate(sampleRate))
                .WithDataSubChunk(x => x
                    .WithChunkSize((uint)newDataSize)
                    .WithData(WaveStretcher.StretchAsync(audio, factor, cancellationToken)));
        }
    }
}
