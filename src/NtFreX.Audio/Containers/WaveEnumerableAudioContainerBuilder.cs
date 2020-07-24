using Dasync.Collections;
using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;

namespace NtFreX.Audio.Containers
{
    public static class WaveEnumerableAudioContainerBuilder
    {
        public static WaveEnumerableAudioContainer Build(AudioFormat format, byte[] data)
        {
            _ = format ?? throw new ArgumentNullException(nameof(format));
            _ = data ?? throw new ArgumentNullException(nameof(data));

            //TODO: fix riff chunk size and make constant for fmt size and ids
            return new WaveEnumerableAudioContainer(
                   new RiffChunkDescriptor(RiffChunkDescriptor.ChunkIdentifierRIFF, /* size of file minus 8: 36 + data in default case */ (uint) (36 + data.Length), RiffChunkDescriptor.WAVE),
                   new FmtSubChunk(FmtSubChunk.ChunkIdentifier, FmtSubChunk.FmtChunkSize, format.Type, format.Channels, format.SampleRate, format.BitsPerSample),
                   new EnumerableDataSubChunk(
                       DataSubChunk.ChunkIdentifer,
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
