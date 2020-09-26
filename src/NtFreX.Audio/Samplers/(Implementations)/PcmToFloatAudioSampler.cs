using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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

            var isLittleEndian = audio.IsDataLittleEndian();
            return Task.FromResult(
                audio.WithData(
                    data: SampleInnerAsync(audio, format, isLittleEndian, cancellationToken)
                        .ToNonSeekable(audio.GetDataLength()),
                    format: new AudioFormat(format.SampleRate, format.BitsPerSample, format.Channels, AudioFormatType.IeeFloat)));
        }

        private static async IAsyncEnumerable<Sample> SampleInnerAsync(ISeekableAsyncEnumerable<Sample> audio, IAudioFormat format, bool isLittleEndian, CancellationToken cancellationToken)
        {
            var max = (System.Math.Pow(2, format.BitsPerSample) / 2) - 1;
            var definition = new SampleDefinition(AudioFormatType.IeeFloat, format.BitsPerSample, isLittleEndian);
            await foreach(var sample in audio)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                yield return new Sample(sample.Value / max, definition);
            }
        }
    }
}
