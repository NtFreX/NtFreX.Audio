using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using System;
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

        public override async Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var format = audio.GetFormat();
            if (format.SampleRate == sampleRate)
            {
                return audio;
            }

            var pipe = new AudioSamplerPipe();
            var currentSampleRate = format.SampleRate;
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

        private IntermediateEnumerableAudioContainer SampleInner(IntermediateEnumerableAudioContainer audio, double factor, CancellationToken cancellationToken = default)
        {
            var format = audio.GetFormat();

            return audio.WithData(
                data: WaveStretcher.StretchAsync(audio, factor, cancellationToken),
                format: new AudioFormat(sampleRate, format.BitsPerSample, format.Channels, format.Type));
        }
    }
}
