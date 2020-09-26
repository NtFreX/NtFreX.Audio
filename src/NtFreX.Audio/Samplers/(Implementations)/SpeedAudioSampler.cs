using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
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

            var newSize = (uint)System.Math.Round(speedFactor * audio.GetDataLength(), 0);

            return Task.FromResult(audio.WithData(
                data: WaveStretcher
                    .StretchAsync(audio, speedFactor, cancellationToken)
                    .ToNonSeekable(newSize)));
        }

        public override string ToString()
        {
            return base.ToString() + $", speedFactor={speedFactor}";
        }
    }
}
