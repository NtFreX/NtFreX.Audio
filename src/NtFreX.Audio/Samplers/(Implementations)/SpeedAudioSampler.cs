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

            // TODO: make work with factor bigger then 2 or smaller then 0.5
            var currentSpeedFactor = speedFactor;
            var newDataSize = System.Math.Round(speedFactor * audio.DataSubChunk.ChunkSize, 0);
            while (currentSpeedFactor > 0)
            {
                audio = audio.WithDataSubChunk(x => x.WithData(WaveStretcher.StretchAsync(audio, speedFactor, cancellationToken)));
                currentSpeedFactor -= 2;
            }

            return Task.FromResult(audio
                .WithDataSubChunk(x => x.WithChunkSize((uint)newDataSize)));
        }

        public override string ToString()
        {
            return base.ToString() + $", speedFactor={speedFactor}";
        }
    }
}
