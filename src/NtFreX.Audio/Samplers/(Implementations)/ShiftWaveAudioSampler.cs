using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure.Threading;
using System;
using System.Diagnostics.CodeAnalysis;
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

        [return: NotNull]
        public override Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var max = (long) System.Math.Pow(256, audio.FmtSubChunk.BitsPerSample / 8) / 2;
            var min = max * -1;

            var samples = audio.GetAudioSamplesAsync(cancellationToken).SelectAsync(x => x + shiftAdder, cancellationToken);
            
            return Task.FromResult(audio.WithDataSubChunk(x => x.WithData(samples)));
        }

        public override string ToString()
        {
            return base.ToString() + $", shiftAdder={shiftAdder}";
        }
    }
}
