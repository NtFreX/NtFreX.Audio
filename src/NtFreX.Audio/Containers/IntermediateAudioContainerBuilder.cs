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
        public static IntermediateEnumerableAudioContainer BuildSilence(IAudioFormat format, int lengthInSeconds, bool isLittleEndian = true)
        {
            var data = WaveBuilder.Silence(format, lengthInSeconds, isLittleEndian);
            return Build(format, data, isLittleEndian);
        }

        public static IntermediateEnumerableAudioContainer Build(IAudioFormat format, IEnumerable<byte> data, ulong? length, bool isLittleEndian = true)
            => Build(format, data.ToAsyncEnumerable(), length, isLittleEndian);

        public static IntermediateEnumerableAudioContainer Build(IAudioFormat format, IAsyncEnumerable<byte> data, ulong? length, bool isLittleEndian = true)
            => Build(format, data.ToNonSeekable(length), isLittleEndian);

        public static IntermediateEnumerableAudioContainer Build(IAudioFormat format, ISeekableAsyncEnumerable<byte> data, bool isLittleEndian = true)
        {
            if(data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if(format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            return Build(
                format,
                data.GroupByLengthAsync(format.BytesPerSample).SelectAsync(x => x.AsMemory()),
                data.CanGetLength() ? (ulong?)data.GetDataLength() : null,
                isLittleEndian);
        }

        public static IntermediateEnumerableAudioContainer Build(IAudioFormat format, ISeekableAsyncEnumerable<Memory<byte>> data, ulong? realByteLength, bool isLittleEndian = true)
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