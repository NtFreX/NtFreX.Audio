using Dasync.Collections;
using NtFreX.Audio.Samplers;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace NtFreX.Audio.Tests
{
    [TestFixture]
    public class MonoToStereoSamplerTest {
        [Test]
        public async Task ShouldHaveTwoChannels()
        {
            var audio = TestHelper.Build(10, 16, 44100, channels: 1);
            var sampler = new FromMonoAudioSampler(2);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            
            Assert.AreEqual(2, newAudio.FmtSubChunk.Channels);
        }
        [Test]
        public async Task ShouldHaveDoubleAmountOfData()
        {
            var audio = TestHelper.Build(10, 16, 44100, channels: 1);
            var sampler = new FromMonoAudioSampler(2);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            var newData = await newAudio.GetAudioSamplesAsync().ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(audio.DataSubChunk.ChunkSize*2, newAudio.DataSubChunk.ChunkSize);
            Assert.AreEqual(newAudio.DataSubChunk.ChunkSize, newData.Sum(x => x.Definition.Bits / 8));
        }

        [Test]
        public async Task ShouldHaveSameLengthAfterSampling()
        {
            var audio = TestHelper.Build(10, 16, 44100, channels: 1);
            var sampler = new FromMonoAudioSampler(2);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);

            Assert.AreEqual(audio.GetLength().Ticks, newAudio.GetLength().Ticks);
        }
    }
}
