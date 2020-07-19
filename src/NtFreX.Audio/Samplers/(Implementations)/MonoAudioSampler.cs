using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Math;
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
    public class MonoAudioSampler : AudioSampler
    {
        [return:NotNull] public override Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if (audio.FmtSubChunk.NumChannels == 1)
            {
                return Task.FromResult(audio);
            }

            var monoData =
                InterleaveChannelData(audio, cancellationToken)
                    .SelectAsync(x => x.ToByteArray(audio.FmtSubChunk.BitsPerSample / 8));

            return Task.FromResult(audio
                    .WithFmtSubChunk(x => x
                        .WithNumChannels(1))
                    .WithDataSubChunk(x => x
                        .WithChunkSize(audio.DataSubChunk.ChunkSize / audio.FmtSubChunk.NumChannels)
                        .WithData(monoData)));
        }

        [return:NotNull] private static async IAsyncEnumerable<long> InterleaveChannelData([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var temp = new long[audio.FmtSubChunk.NumChannels];
            var counter = 0;
            var samples = audio.DataSubChunk.Data;
            await foreach (var value in samples.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                temp[counter++] = value.ToInt64();
                if (counter == audio.FmtSubChunk.NumChannels)
                {
                    yield return (long)temp.Average();
                    counter = 0;
                }
            }
        }
    }
}
