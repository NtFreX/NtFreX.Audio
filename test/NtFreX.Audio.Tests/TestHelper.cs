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
                   new FmtSubChunk("fmt ", 16, AudioFormatType.Pcm, channels, sampleRate, bitsPerSample),
                   new EnumerableDataSubChunk(
                       "data",
                       (uint)byteCount,
                       Enumerable.Repeat(0L, (int)totalSamples).Select(x => x.ToByteArray(bitsPerSample / 8)).ToAsyncEnumerable()),
                   new List<UnknownSubChunk>());
        }

        public static Sample[] BuildChannelSamples(ushort bitsPerSample, params long[] channelSamples)
        {
            var buffer = new Sample[channelSamples.Length];
            for (var i = 0; i < channelSamples.Length; i++)
            {
                buffer[i] = new Sample(channelSamples[i].ToByteArray(bitsPerSample / 8), new SampleDefinition(AudioFormatType.Pcm, bitsPerSample, isLittleEndian: true));
            }
            return buffer;
        }
        public static void AssertChannelAverage(int channelToTest, Sample[] resultChannelSamples, ushort bitsPerSample, params long[] expectedValues)
        {
            var channelValue = resultChannelSamples.SelectMany(x => x.AsByteArray()).Skip(channelToTest * bitsPerSample / 8).Take(bitsPerSample / 8).ToArray().ToInt64();
            Assert.AreEqual((long)System.Math.Round(expectedValues.Sum() / (double)expectedValues.Length, 0), channelValue);
        }
    }
}
