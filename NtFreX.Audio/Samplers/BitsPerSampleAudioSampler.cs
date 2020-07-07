using NtFreX.Audio.Extensions;
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

        //TODO: make this work correctly with and from all sample rates
        /// <summary>
        /// 
        /// audio will be disposed
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="onProgress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [return:NotNull] public override Task<WaveAudioContainerStream> SampleAsync([NotNull] WaveAudioContainerStream audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if (audio.Container.FmtSubChunk.BitsPerSample == bitsPerSample)
            {
                return Task.FromResult(audio);
            }

            var max = System.Math.Pow(256, audio.Container.FmtSubChunk.BitsPerSample / 8) / 2;
            var newMax = System.Math.Pow(256, bitsPerSample / 8) / 2;
            var factor = max / newMax;

            var samples = audio.Stream.SelectAsync(x => ((long)(x.ToInt64() / factor)).ToByteArray(bitsPerSample / 8));

#pragma warning disable CA2000 // Dispose objects before losing scope
            return Task.FromResult(new WaveAudioContainerStream(audio.Container
                .WithFmtSubChunk(x => x
                    .WithBitsPerSample(bitsPerSample))
                .WithDataSubChunk(x => x
                    .WithSubchunk2Size(audio.Container.DataSubChunk.Subchunk2Size / audio.Container.FmtSubChunk.BitsPerSample * bitsPerSample)), samples));
#pragma warning restore CA2000 // Dispose objects before losing scope
        }

        public override string ToString()
        {
            return base.ToString() + $", bitsPerSample={bitsPerSample}";
        }
    }
}
