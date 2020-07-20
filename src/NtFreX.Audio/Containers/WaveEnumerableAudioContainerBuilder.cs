using Dasync.Collections;
using NtFreX.Audio.AdapterInfrastructure;
using System.Collections.Generic;

namespace NtFreX.Audio.Containers
{
    public static class WaveEnumerableAudioContainerBuilder
    {
        public static WaveEnumerableAudioContainer Build(AudioFormat format, byte[] data)
        {
            return new WaveEnumerableAudioContainer(
                   new RiffChunkDescriptor(RiffChunkDescriptor.ChunkIdentifierRIFF, 5057218, RiffChunkDescriptor.WAVE),
                   new FmtSubChunk("fmt ", 16, (ushort)format.Type, format.Channels, format.SampleRate, format.BitsPerSample),
                   new EnumerableDataSubChunk(
                       "data",
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
