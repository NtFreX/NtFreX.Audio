using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class FloatToPcmAudioSampler : AudioSampler
    {
        public override Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var format = audio.GetFormat();
            if (format.Type == AudioFormatType.Pcm)
            {
                return Task.FromResult(audio);
            }
            if (format.Type != AudioFormatType.IeeFloat)
            {
                throw new ArgumentException("The given format must be float", nameof(audio));
            }

            var max = (System.Math.Pow(2, format.BitsPerSample) / 2) - 1;
            var definition = new SampleDefinition(AudioFormatType.Pcm, format.BitsPerSample, audio.IsDataLittleEndian());

            return Task.FromResult(audio.WithData(
                data: audio.SelectAsync(x => FloatToPcm(x, max, definition), cancellationToken),
                format: new AudioFormat(format.SampleRate, format.BitsPerSample, format.Channels, AudioFormatType.Pcm)));
        }

        private static Sample FloatToPcm(Sample sample, double max, SampleDefinition definition)
            => new Sample(sample.Value * max, definition);
    }
}
