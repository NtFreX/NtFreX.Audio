using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
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
            return Build(format, data, isLittleEndian);
        }

        public static IntermediateEnumerableAudioContainer Build(IAudioFormat format, byte[] data, bool isLittleEndian = true)
            => Build(format, data.ToAsyncEnumerable(), data?.Length ?? throw new ArgumentNullException(nameof(data)), isLittleEndian);

        public static IntermediateEnumerableAudioContainer Build(IAudioFormat format, IEnumerable<byte> data, long length, bool isLittleEndian = true)
            => Build(format, data.ToAsyncEnumerable(), length, isLittleEndian);

        public static IntermediateEnumerableAudioContainer Build(IAudioFormat format, IAsyncEnumerable<byte> data, long length, bool isLittleEndian = true)
            => Build(format, data.ToNonSeekable(length), isLittleEndian);

        public static IntermediateEnumerableAudioContainer Build(IAudioFormat format, ISeekableAsyncEnumerable<byte> data, bool isLittleEndian = true)
            => Build(
                format, 
                data.GroupByLengthAsync(format?.BytesPerSample ?? throw new ArgumentNullException(nameof(format))).SelectAsync(x => x.AsMemory()),
                data?.GetDataLength() ?? throw new ArgumentNullException(nameof(data)),
                isLittleEndian);

        public static IntermediateEnumerableAudioContainer Build(IAudioFormat format, ISeekableAsyncEnumerable<Memory<byte>> data, long realByteLength, bool isLittleEndian = true)
        {
            _ = format ?? throw new ArgumentNullException(nameof(format));
            _ = data ?? throw new ArgumentNullException(nameof(data));

            var enumerable = data.ToSamplesAsync(realByteLength, format, isLittleEndian);

            return new IntermediateEnumerableAudioContainer(
                enumerable,
                format,
                isLittleEndian);
        }
    }
}