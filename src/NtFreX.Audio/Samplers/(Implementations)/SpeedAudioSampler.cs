using NtFreX.Audio.Containers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class SpeedAudioSampler : AudioSampler
    {
        private readonly double speedFactor;

        public SpeedAudioSampler(double speedFactor)
        {
            this.speedFactor = speedFactor;
        }

        public override Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            return Task.FromResult(audio.WithData(
                data: WaveStretcher.StretchAsync(audio, speedFactor, cancellationToken)));
        }

        public override string ToString()
        {
            return base.ToString() + $", speedFactor={speedFactor}";
        }
    }
}
