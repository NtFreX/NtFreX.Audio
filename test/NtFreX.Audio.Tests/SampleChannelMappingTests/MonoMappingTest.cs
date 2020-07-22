using NtFreX.Audio.Samplers;
using NUnit.Framework;
using static NtFreX.Audio.Tests.TestHelper;

namespace NtFreX.Audio.Tests.SampleChannelMappingTests
{
    [TestFixture]
    public class MonoMappingTest
    {
        [TestCase(8U)]
        [TestCase(16U)]
        [TestCase(32U)]
        [TestCase(64U)]
        public void SampleToStereoTest(uint bitsPerSample)
        {
            var sampleChannelMapping = new MonoSampleChannelMapping();
            byte[] sample = BuildChannelSamples(
                (ushort)bitsPerSample,
                2 // center
            );

            var toStereoSample = sampleChannelMapping.ToStereo(sample, (ushort)bitsPerSample);

            AssertChannelAverage(0, toStereoSample, (ushort)bitsPerSample, 2);
        }

    }
}
