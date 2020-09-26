using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Collections.Generic;
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

        public override Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var format = audio.GetFormat();
            if (format.BitsPerSample == bitsPerSample)
            {
                return Task.FromResult(audio);
            }

            //TODO: make this work correctly with and from all sample rates
            // HINT: doubling bits per sample and not changing data will double speed
            var isNewBigger = bitsPerSample > format.BitsPerSample;
            var factor = System.Math.Pow(256, isNewBigger ? bitsPerSample / format.BitsPerSample : format.BitsPerSample / bitsPerSample);
            var definition = new SampleDefinition(format.Type, bitsPerSample, audio.IsDataLittleEndian());

            return Task.FromResult(audio.WithData(
                data: audio.SelectAsync(sample => Selector(sample, format, isNewBigger, factor, definition), cancellationToken),
                format: new AudioFormat(format.SampleRate, bitsPerSample, format.BitsPerSample, format.Type)));
        }

        public override string ToString()
        {
            return base.ToString() + $", bitsPerSample={bitsPerSample}";
        }

        private Sample Selector(Sample sample, IAudioFormat format, bool isNewBigger, double factor, SampleDefinition definition)
        {
            return new Sample(
                format.Type switch
                {
                    AudioFormatType.Pcm => isNewBigger ? sample.Value * factor : sample.Value / factor,
                    AudioFormatType.IeeFloat => sample.Value,
                    _ => throw new Exception()
                }, definition);
        }
    }
}
