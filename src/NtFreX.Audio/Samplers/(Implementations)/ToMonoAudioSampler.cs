using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    /// <summary>
    /// converts stereo to mono 
    /// </summary>
    public class ToMonoAudioSampler : AudioSampler
    {
        [return:NotNull] public override Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if (audio.FmtSubChunk.Channels == 1)
            {
                return Task.FromResult(audio);
            }

            return Task.FromResult(audio
                    .WithFmtSubChunk(x => x
                        .WithChannels(1))
                    .WithDataSubChunk(x => x
                        .WithChunkSize(audio.DataSubChunk.ChunkSize / audio.FmtSubChunk.Channels)
                        .WithData(InterleaveChannelData(audio, cancellationToken))));
        }

        [return:NotNull] private static async IAsyncEnumerable<Sample> InterleaveChannelData([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var temp = new Sample[audio.FmtSubChunk.Channels];
            var counter = 0;
            var samples = audio.GetAudioSamplesAsync(cancellationToken);
            await foreach (var value in samples.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                temp[counter++] = value;
                if (counter == audio.FmtSubChunk.Channels)
                {
                    yield return temp.Average();
                    counter = 0;
                }
            }
        }
    }
}
