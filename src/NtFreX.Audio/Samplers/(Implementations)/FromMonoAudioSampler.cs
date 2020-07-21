﻿using NtFreX.Audio.Containers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class FromMonoAudioSampler : AudioSampler
    {
        private readonly int targetChannels;

        public FromMonoAudioSampler(int targetChannels)
        {
            this.targetChannels = targetChannels;
        }

        [return: NotNull]
        public override Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if(audio.FmtSubChunk.Channels != 1)
            {
                throw new ArgumentException("Only mono is supported");
            }

            if (audio.FmtSubChunk.Channels == targetChannels)
            {
                return Task.FromResult(audio);
            }
            
            return Task.FromResult(audio
                    .WithFmtSubChunk(x => x
                        .WithChannels(2))
                    .WithDataSubChunk(x => x
                        .WithChunkSize((uint)(audio.DataSubChunk.ChunkSize * targetChannels))
                        .WithData(DuplicateChannelData(audio, cancellationToken))));
        }

        [return: NotNull]
        private async IAsyncEnumerable<byte[]> DuplicateChannelData([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var samples = audio.DataSubChunk.Data;
            await foreach (var value in samples.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                for (var i = 0; i < targetChannels; i++)
                {
                    yield return value;
                }
            }
        }
    }
}