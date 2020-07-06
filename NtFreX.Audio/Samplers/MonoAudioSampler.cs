using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Helpers;
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
    public class MonoAudioSampler : AudioSampler
    {
        [return:NotNull] public override async Task<WaveAudioContainer> SampleAsync([NotNull] WaveAudioContainer audio, [NotNull] Action<double> onProgress, [MaybeNull] CancellationToken cancellationToken = default)
        {
            if (audio.FmtSubChunk.NumChannels == 1)
            {
                return audio;
            }

            IAsyncEnumerable<byte> monoData = 
                InterleaveChannelData(audio, cancellationToken)
                    .SelectAsync(x => x.ToByteArray(audio.FmtSubChunk.BitsPerSample / 8))
                    .SelectManyAsync(x => x);

            var monoStream = await StreamFactory.Instance.WriteToNewStreamAsync(
                monoData, 
                AsRelativeProgress(onProgress, audio.DataSubChunk.Subchunk2Size / (double)audio.FmtSubChunk.NumChannels), 
                cancellationToken);

            return audio
                .WithFmtSubChunk(x => x
                    .WithNumChannels(1))
                .WithDataSubChunk(x => x
                    .WithData(monoStream)
                    .WithSubchunk2Size((uint)monoStream.Length));
        }

        [return:NotNull] private async IAsyncEnumerable<long> InterleaveChannelData([NotNull] WaveAudioContainer audio, [MaybeNull] [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var temp = new long[audio.FmtSubChunk.NumChannels];
            var counter = 0;
            var samples = audio.GetAudioSamplesAsync(cancellationToken);
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
