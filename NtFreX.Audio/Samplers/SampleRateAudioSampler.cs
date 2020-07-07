using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    //public class VolumeAudioSampler : AudioSampler { } // strech out wave (height)
    //public class VolumeNormalizerAudioSampler : AudioSampler { }
    //public class SpeedAudioSampler: AudioSampler { } // strech out wave (width)
    //public class BackgroundNoiseAudioSampler: AudioSampler { } //TODO: cache fourier transform
    //public class ShitWaveAudioSampler: AudioSampler {} // shift wave up or down

    //public class SplitAudio {} 
    //public class JoinAudio {} 
    //public class ConvertAudio {}

    public class SampleRateAudioSampler : AudioSampler
    {
        private readonly uint sampleRate;

        public SampleRateAudioSampler(uint sampleRate)
        {
            this.sampleRate = sampleRate;
        }

        [return:NotNull] public override Task<WaveAudioContainerStream> SampleAsync([NotNull] WaveAudioContainerStream audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            if (audio.Container.FmtSubChunk.SampleRate == sampleRate)
            {
                return Task.FromResult(audio);
            }

            // TODO: implement
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString() + $", sampleRate={sampleRate}";
        }
    }
}
