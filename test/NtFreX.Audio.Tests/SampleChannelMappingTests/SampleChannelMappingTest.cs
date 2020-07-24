using Dasync.Collections;
using NtFreX.Audio.Samplers;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace NtFreX.Audio.Tests.SampleChannelMappingTests
{
    [TestFixture]
    public class SampleChannelMappingTest
    {
        [TestCase(2U, 1U)]
        [TestCase(1U, 2U)]
        [TestCase(3U, 4U)]
        [TestCase(4U, 6U)]
        [TestCase(5U, 4U)]
        [TestCase(8U, 6U)]
        public async Task ShouldHaveCorrectChannels(uint srcChannel, uint trgChannel)
        {
            ushort sourceChannels = (ushort)srcChannel;
            ushort targetChannels = (ushort)trgChannel;
            var audio = TestHelper.Build(10, 16, 44100, channels: sourceChannels);
            var sampler = new ChannelAudioSampler(targetChannels);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);

            Assert.AreEqual(targetChannels, newAudio.FmtSubChunk.Channels);
        }

        [TestCase(1U, 2U)]
        [TestCase(3U, 4U)]
        [TestCase(4U, 6U)]
        public async Task ShouldHaveCorrectAmountOfData(uint srcChannel, uint trgChannel)
        {
            ushort sourceChannels = (ushort)srcChannel;
            ushort targetChannels = (ushort)trgChannel;
            var audio = TestHelper.Build(10, 16, 44100, channels: sourceChannels);
            var sampler = new ChannelAudioSampler(targetChannels);
            var factor = targetChannels / (double) sourceChannels;

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            var newData = await newAudio.GetAudioSamplesAsync().ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(audio.DataSubChunk.ChunkSize * factor, newAudio.DataSubChunk.ChunkSize);
            Assert.AreEqual(newAudio.DataSubChunk.ChunkSize, newData.Sum(x => x.Definition.Bits / 8));
        }

        [TestCase(2U, 1U)]
        [TestCase(1U, 2U)]
        [TestCase(3U, 4U)]
        [TestCase(4U, 6U)]
        [TestCase(5U, 4U)]
        [TestCase(8U, 6U)]
        public async Task ShouldHaveSameLengthAfterSampling(uint srcChannel, uint trgChannel)
        {
            ushort sourceChannels = (ushort) srcChannel;
            ushort targetChannels = (ushort) trgChannel;

            var audio = TestHelper.Build(10, 16, 44100, channels: sourceChannels);
            var sampler = new ChannelAudioSampler(targetChannels);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);

            Assert.AreEqual(audio.GetLength().Ticks, newAudio.GetLength().Ticks);
        }
    }
}
