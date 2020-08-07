using NtFreX.Audio.Infrastructure;
using NUnit.Framework;
using System.Linq;

namespace NtFreX.Audio.Tests
{
    internal static class TestHelper
    {
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
