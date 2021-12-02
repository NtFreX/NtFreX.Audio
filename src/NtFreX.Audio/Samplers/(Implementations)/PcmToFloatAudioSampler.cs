using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class PcmToFloatAudioSampler : AudioSampler
    {
        public override Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var format = audio.GetFormat();
            if (format.Type == AudioFormatType.IeeFloat)
            {
                return Task.FromResult(audio);
            }
            if (format.Type != AudioFormatType.Pcm)
            {
                throw new ArgumentException("The given format must be pcm", nameof(audio));
            }

            var max = (System.Math.Pow(2, format.BitsPerSample) / 2) - 1;
            var definition = new SampleDefinition(AudioFormatType.IeeFloat, format.BitsPerSample, audio.IsDataLittleEndian());

            return Task.FromResult(
                audio.WithData(
                    data: audio.SelectAsync(x => PcmToFloat(x, max, definition), cancellationToken),
                    format: new AudioFormat(format.SampleRate, format.BitsPerSample, format.Channels, AudioFormatType.IeeFloat)));
        }

        private static Sample PcmToFloat(Sample sample, double max, SampleDefinition definition)
            => new Sample(sample.AsNumber() / max, definition);
    }
}
