using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using NtFreX.Audio.Math;
using System;
using System.Collections.Generic;

namespace NtFreX.Audio.Containers
{
    public static class IntermediateAudioContainerBuilder
    {
        public static IntermediateEnumerableAudioContainer Build(IAudioFormat format, int lengthInSeconds, bool isLittleEndian = true)
        {
            var data = WaveBuilder.Silence(format, lengthInSeconds, isLittleEndian);
            return Build(format, data, lengthInSeconds * format.SampleRate * format.BytesPerSample * format.Channels, isLittleEndian);
        }

        public static IntermediateEnumerableAudioContainer Build(IAudioFormat format, IEnumerable<byte> data, long length, bool isLittleEndian = true)
            => Build(format, data.ToAsyncEnumerable(), length, isLittleEndian);

        public static IntermediateEnumerableAudioContainer Build(IAudioFormat format, IAsyncEnumerable<byte> data, long length, bool isLittleEndian = true)
        {
            _ = format ?? throw new ArgumentNullException(nameof(format));
            _ = data ?? throw new ArgumentNullException(nameof(data));

            var enumerable = data
                .GroupByLengthAsync(format.BytesPerSample)
                .ToAudioSamplesAsync(new SampleDefinition(format.Type, format.BitsPerSample, isLittleEndian))
                .ToNonSeekable(length / format.BytesPerSample);

            return new IntermediateEnumerableAudioContainer(
                enumerable,
                format,
                isLittleEndian);
        }
    }
}