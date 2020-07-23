using NtFreX.Audio.Samplers;
using NUnit.Framework;

using static NtFreX.Audio.Tests.TestHelper;

namespace NtFreX.Audio.Tests.SampleChannelMappingTests
{
    [TestFixture]
    public class ThreePointOneMappingTests
    {
        [TestCase(8U)]
        [TestCase(16U)]
        [TestCase(32U)]
        [TestCase(64U)]
        public void SampleToMonoTest(uint bitsPerSample)
        {
            var sampleChannelMapping = new ThreePointOneSampleChannelMapping();
            var sample = BuildChannelSamples(
                (ushort)bitsPerSample,
                2, // left
                3, // right
                5, // center
                1 /* low frequency */ );

            var toMonoSample = sampleChannelMapping.ToMono(sample);

            AssertChannelAverage(0, toMonoSample, (ushort)bitsPerSample, 2, 3, 5, 1);
        }

        [TestCase(8U)]
        [TestCase(16U)]
        [TestCase(32U)]
        [TestCase(64U)]
        public void SampleToStereoTest(uint bitsPerSample)
        {
            var sampleChannelMapping = new ThreePointOneSampleChannelMapping();
            var sample = BuildChannelSamples(
                (ushort)bitsPerSample,
                2, // left
                3, // right
                5, // center
                1 /* low frequency */ );

            var toStereoSample = sampleChannelMapping.ToStereo(sample);

            AssertChannelAverage(0, toStereoSample, (ushort)bitsPerSample, 2, 5, 1); // front left
            AssertChannelAverage(1, toStereoSample, (ushort)bitsPerSample, 3, 5, 1); // front right
        }
    }
}
