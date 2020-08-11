using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Math;
using System;
using System.Collections.Generic;

namespace NtFreX.Audio.Containers
{
    public static class WaveEnumerableAudioContainerBuilder
    {
        public static WaveEnumerableAudioContainer Build(IAudioFormat format, int lengthInSeconds, bool isLittleEndian = true)
        {
            var data = WaveBuilder.Silence(format, lengthInSeconds, isLittleEndian);
            return Build(format, data, isLittleEndian);
        }

        public static WaveEnumerableAudioContainer Build(IAudioFormat format, byte[] data, bool isLittleEndian = true)
        {
            _ = format ?? throw new ArgumentNullException(nameof(format));
            _ = data ?? throw new ArgumentNullException(nameof(data));

            return new WaveEnumerableAudioContainer(
                   new RiffSubChunk(isLittleEndian ? RiffSubChunk.ChunkIdentifierRIFF : RiffSubChunk.ChunkIdentifierRIFX, /* size of file minus 8: 36 + data in default case */ (uint) (WaveAudioContainer<IDataSubChunk>.DefaultHeaderSize + data.Length), RiffSubChunk.WAVE),
                   new FmtSubChunk(FmtSubChunk.ChunkIdentifier, FmtSubChunk.FmtChunkSize, format.Type, format.Channels, format.SampleRate, format.BitsPerSample),
                   new EnumerableDataSubChunk(
                       DataSubChunk<ISubChunk>.ChunkIdentifer,
                       (uint)data.Length,
                       GroupByLength(data, format.BitsPerSample / 8).ToAsyncEnumerable()),
                   new List<UnknownSubChunk>());
        }

        private static IEnumerable<byte[]> GroupByLength(byte[] data, int length)
        {
            var buffer = new byte[length];
            var index = 0;
            foreach (var value in data)
            {
                buffer[index++] = value;
                if (index == length)
                {
                    yield return buffer;
                    buffer = new byte[length];
                    index = 0;
                }
            }
        }
    }
}
