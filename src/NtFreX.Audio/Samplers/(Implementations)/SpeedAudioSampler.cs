using NtFreX.Audio.Containers;
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

        [return: NotNull]
        public override Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            return Task.FromResult(audio
                .WithDataSubChunk(x => x
                    .WithChunkSize((uint)System.Math.Round(speedFactor * audio.DataSubChunk.ChunkSize, 0))
                    .WithData(WaveStretcher.StretchAsync(audio, speedFactor, cancellationToken))));
        }

        public override string ToString()
        {
            return base.ToString() + $", speedFactor={speedFactor}";
        }
    }
}
