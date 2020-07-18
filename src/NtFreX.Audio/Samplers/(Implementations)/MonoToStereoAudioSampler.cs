using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class MonoToStereoAudioSampler : AudioSampler
    {

        [return: NotNull]
        public override Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if (audio.FmtSubChunk.NumChannels == 2)
            {
                return Task.FromResult(audio);
            } 
            else if (audio.FmtSubChunk.NumChannels != 1)
            {
                throw new ArgumentException("can only convert Mono to Stereo!", nameof(audio));
            }

            var stereoData =
               DuplicateChannelData(audio, cancellationToken)
                   .SelectAsync(x => x.ToByteArray(audio.FmtSubChunk.BitsPerSample / 8));

            return Task.FromResult(audio
                    .WithFmtSubChunk(x => x
                        .WithNumChannels(2))
                    .WithDataSubChunk(x => x
                        .WithSubchunk2Size(audio.DataSubChunk.Subchunk2Size * 2)
                        .WithData(stereoData)));
        }

        [return: NotNull]
        private static async IAsyncEnumerable<long> DuplicateChannelData([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var samples = audio.DataSubChunk.Data;
            await foreach (var value in samples.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return value.ToInt64();
                yield return value.ToInt64();
            }
        }
    }
}
