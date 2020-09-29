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
                buffer[i] = new Sample(channelSamples[i], new SampleDefinition(AudioFormatType.Pcm, bitsPerSample, isLittleEndian: true));
            }
            return buffer;
        }
        public static void AssertChannelAverage(int channelToTest, Sample[] resultChannelSamples, ushort bitsPerSample, params long[] expectedValues)
        {
            var channelValue = Number.FromGivenBits(resultChannelSamples.SelectMany(x => x.AsByteArray().ToArray()).Skip(channelToTest * bitsPerSample / 8).Take(bitsPerSample / 8).ToArray());
            Assert.AreEqual(System.Math.Round(expectedValues.Sum() / (double)expectedValues.Length, 0), channelValue);
        }
    }
}
