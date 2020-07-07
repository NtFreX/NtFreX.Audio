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
    public class MonoAudioSampler : AudioSampler
    {
        /// <summary>
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [return:NotNull] public override Task<WaveAudioContainerStream> SampleAsync([NotNull] WaveAudioContainerStream audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if (audio.Container.FmtSubChunk.NumChannels == 1)
            {
                return Task.FromResult(audio);
            }

            IAsyncEnumerable<byte[]> monoData =
                InterleaveChannelData(audio, cancellationToken)
                    .SelectAsync(x => x.ToByteArray(audio.Container.FmtSubChunk.BitsPerSample / 8));

#pragma warning disable CA2000 // Dispose objects before losing scope
            return Task.FromResult(new WaveAudioContainerStream(
                audio.Container
                    .WithFmtSubChunk(x => x
                        .WithNumChannels(1))
                    .WithDataSubChunk(x => x
                        .WithSubchunk2Size(audio.Container.DataSubChunk.Subchunk2Size / audio.Container.FmtSubChunk.NumChannels)), monoData));
#pragma warning restore CA2000 // Dispose objects before losing scope
        }

        [return:NotNull] private async IAsyncEnumerable<long> InterleaveChannelData([NotNull] WaveAudioContainerStream audio, [MaybeNull] [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var temp = new long[audio.Container.FmtSubChunk.NumChannels];
            var counter = 0;
            var samples = audio.Stream;
            await foreach (var value in samples.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                temp[counter++] = value.ToInt64();
                if (counter == audio.Container.FmtSubChunk.NumChannels)
                {
                    yield return (long)temp.Average();
                    counter = 0;
                }
            }
        }
    }
}
