using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
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
            var audio = IntermediateAudioContainerBuilder.Build(new AudioFormat(44100, 16, sourceChannels, AudioFormatType.Pcm), lengthInSeconds: 10);
            var sampler = new ChannelAudioSampler(targetChannels);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);

            Assert.AreEqual(targetChannels, newAudio.GetFormat().Channels);
        }

        [TestCase(1U, 2U)]
        [TestCase(3U, 4U)]
        [TestCase(4U, 6U)]
        public async Task ShouldHaveCorrectAmountOfData(uint srcChannel, uint trgChannel)
        {
            ushort sourceChannels = (ushort)srcChannel;
            ushort targetChannels = (ushort)trgChannel;
            var audio = IntermediateAudioContainerBuilder.Build(new AudioFormat(44100, 16, sourceChannels, AudioFormatType.Pcm), lengthInSeconds: 10);
            var sampler = new ChannelAudioSampler(targetChannels);
            var factor = targetChannels / (double) sourceChannels;

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            var newData = await newAudio.ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(audio.GetDataLength() * factor, newAudio.GetDataLength());
            Assert.AreEqual(newAudio.GetByteLength(), newData.Sum(x => x.Definition.Bytes));
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

            var audio = IntermediateAudioContainerBuilder.Build(new AudioFormat(44100, 16, sourceChannels, AudioFormatType.Pcm), lengthInSeconds: 10);
            var sampler = new ChannelAudioSampler(targetChannels);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);

            Assert.AreEqual(audio.GetLength().Ticks, newAudio.GetLength().Ticks);
        }
    }
}
