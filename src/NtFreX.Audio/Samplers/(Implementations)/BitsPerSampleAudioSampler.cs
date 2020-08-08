using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
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

        [return:NotNull] public override Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if (audio.FmtSubChunk.BitsPerSample == bitsPerSample)
            {
                return Task.FromResult(audio);
            }

            //TODO: make this work correctly with and from all sample rates
            // HINT: doubling bits per sample and not changing data will double speed
            var isNewBigger = bitsPerSample > audio.FmtSubChunk.BitsPerSample;
            var factor = System.Math.Pow(256, isNewBigger ? bitsPerSample / audio.FmtSubChunk.BitsPerSample : audio.FmtSubChunk.BitsPerSample / bitsPerSample);
            var isLittleEndian = audio.IsDataLittleEndian();
            var samples = audio.GetAudioSamplesAsync().SelectAsync(x => new Sample(x.Value, new SampleDefinition(x.Definition.Type, bitsPerSample, x.Definition.IsLittleEndian))).SelectAsync(x => UpOrDown(audio, x, isNewBigger, factor));

            var newSize = audio.DataSubChunk.ChunkSize / audio.FmtSubChunk.BitsPerSample * bitsPerSample;
            
            return Task.FromResult(audio
                .WithFmtSubChunk(x => x
                    .WithBitsPerSample(bitsPerSample))
                .WithDataSubChunk(x => x
                    .WithChunkSize(newSize)
                    .WithData(samples)));
        }

        public override string ToString()
        {
            return base.ToString() + $", bitsPerSample={bitsPerSample}";
        }

        private static Sample UpOrDown(WaveEnumerableAudioContainer audio, Sample sample, bool isNewBigger, double factor)
            => audio.Format.Type == AudioFormatType.Pcm ? isNewBigger ? sample * factor : sample / factor :
               audio.Format.Type == AudioFormatType.IeeFloat ? sample :
               throw new Exception();
    }
}
