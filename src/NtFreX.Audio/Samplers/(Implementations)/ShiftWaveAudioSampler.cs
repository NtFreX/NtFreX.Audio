using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class ShiftWaveAudioSampler: AudioSampler
    {
        private readonly double shiftAdder;

        public ShiftWaveAudioSampler(double shiftAdder)
        {
            this.shiftAdder = shiftAdder;
        }

        public override Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var format = audio.GetFormat();
            var max = (long) System.Math.Pow(256, format.BytesPerSample) / 2;
            var min = max * -1;

            return Task.FromResult(
                audio.WithData(
                    data: audio.SelectAsync(x => x + shiftAdder, cancellationToken)));
        }

        public override string ToString()
        {
            return base.ToString() + $", shiftAdder={shiftAdder}";
        }
    }
}
