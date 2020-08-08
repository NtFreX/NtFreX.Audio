﻿using NtFreX.Audio.Containers;
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

            var newSize = (uint)System.Math.Round(speedFactor * audio.DataSubChunk.ChunkSize, 0);
            
            return Task.FromResult(audio
                //.WithRiffChunkDescriptor(x => x
                //    .WithChunkSize(audio.RiffChunk.ChunkSize + (newSize - audio.DataSubChunk.ChunkSize)))
                .WithDataSubChunk(x => x
                    .WithChunkSize(newSize)
                    .WithData(WaveStretcher.StretchAsync(audio, speedFactor, cancellationToken))));
        }

        public override string ToString()
        {
            return base.ToString() + $", speedFactor={speedFactor}";
        }
    }
}
