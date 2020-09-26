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

            return Task.FromResult(audio.WithData(
                data: SampleInnerAsync(audio, format, audio.IsDataLittleEndian(), cancellationToken)
                    .ToNonSeekable(audio.GetDataLength()),
                format: new AudioFormat(format.SampleRate, format.BitsPerSample, format.Channels, AudioFormatType.Pcm)));
        }

        private static async IAsyncEnumerable<Sample> SampleInnerAsync(ISeekableAsyncEnumerable<Sample> audio, IAudioFormat format, bool isDataLittleEndian, CancellationToken cancellationToken)
        {
            var max = (System.Math.Pow(2, format.BitsPerSample) / 2) - 1;
            var defintition = new SampleDefinition(AudioFormatType.Pcm, format.BitsPerSample, isDataLittleEndian);
            await foreach(var sample in audio)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                yield return new Sample(sample.Value * max, defintition);
            }
        }
    }
}
