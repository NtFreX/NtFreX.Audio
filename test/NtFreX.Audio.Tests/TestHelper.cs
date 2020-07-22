using Dasync.Collections;
using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Math;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NtFreX.Audio.Tests
{
    internal static class TestHelper
    {
        public static WaveEnumerableAudioContainer Build(int sampleCount, ushort bitsPerSample, uint sampleRate, ushort channels = 1, string riffChunkId = RiffChunkDescriptor.ChunkIdentifierRIFF)
        {
            //TODO use WaveEnumerableAudioContainerBuilder delete this
            var byteCount = sampleRate * sampleCount * bitsPerSample / 8 * channels;
            var totalSamples = sampleRate * sampleCount * channels;
            return new WaveEnumerableAudioContainer(
                   new RiffChunkDescriptor(riffChunkId, 0, RiffChunkDescriptor.WAVE),
                   new FmtSubChunk("fmt ", 16, AudioFormatType.PCM, channels, sampleRate, bitsPerSample),
                   new EnumerableDataSubChunk(
                       "data",
                       (uint)byteCount,
                       Enumerable.Repeat(0L, (int)totalSamples).Select(x => x.ToByteArray(bitsPerSample / 8)).ToAsyncEnumerable()),
                   new List<UnknownSubChunk>());
        }

        public static byte[] BuildChannelSamples(ushort bitsPerSample, params long[] channelSamples)
        {
            var buffer = new byte[channelSamples.Length * bitsPerSample / 8];
            for (var i = 0; i < channelSamples.Length; i++)
            {
                Array.Copy(
                    channelSamples[i].ToByteArray(bitsPerSample / 8),
                    0,
                    buffer,
                    i * bitsPerSample / 8,
                    bitsPerSample / 8);
            }
            return buffer;
        }
        public static void AssertChannelAverage(int channelToTest, byte[] resultChannelSamples, ushort bitsPerSample, params long[] expectedValues)
        {
            var channelValue = resultChannelSamples.Skip(channelToTest * bitsPerSample / 8).Take(bitsPerSample / 8).ToArray().ToInt64();
            Assert.AreEqual((long)System.Math.Round(expectedValues.Sum() / (double)expectedValues.Length, 0), channelValue);
        }
    }
}
