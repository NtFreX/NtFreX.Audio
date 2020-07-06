using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Helpers;
using NtFreX.Audio.Math;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class BitsPerSampleAudioSampler : AudioSampler
    {
        private readonly ushort bitsPerSample;

        public BitsPerSampleAudioSampler(ushort bitsPerSample)
        {
            this.bitsPerSample = bitsPerSample;
        }

        [return:NotNull] public override async Task<WaveAudioContainer> SampleAsync([NotNull] WaveAudioContainer audio, [NotNull] Action<double> onProgress, [MaybeNull] CancellationToken cancellationToken = default)
        {
            if (audio.FmtSubChunk.BitsPerSample == bitsPerSample)
            {
                return audio;
            }

            var max = System.Math.Pow(256, audio.FmtSubChunk.BitsPerSample / 8) / 2;
            var newMax = System.Math.Pow(256, bitsPerSample / 8) / 2;
            var factor = max / newMax;

            var samples =
                audio.GetAudioSamplesAsync(cancellationToken)
                    .SelectAsync(x => ((long)(x.ToInt64() / factor)).ToByteArray(bitsPerSample / 8))
                    .SelectManyAsync(x => x);

            var sampledStream = await StreamFactory.Instance.WriteToNewStreamAsync(
                samples,
                AsRelativeProgress(onProgress, bitsPerSample / (double) audio.FmtSubChunk.BitsPerSample * audio.DataSubChunk.Subchunk2Size), 
                cancellationToken);

            return audio
                .WithFmtSubChunk(x => x
                    .WithBitsPerSample(bitsPerSample))
                .WithDataSubChunk(x => x
                    .WithData(sampledStream)
                    .WithSubchunk2Size((uint)sampledStream.Length));
        }
    }
}
