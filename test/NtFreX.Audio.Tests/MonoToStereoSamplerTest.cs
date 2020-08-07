using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
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
            var audio = WaveEnumerableAudioContainerBuilder.Build(new AudioFormat(44100, 16, 1, AudioFormatType.Pcm), lengthInSeconds: 10);
            var sampler = new FromMonoAudioSampler(2);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            
            Assert.AreEqual(2, newAudio.FmtSubChunk.Channels);
        }
        [Test]
        public async Task ShouldHaveDoubleAmountOfData()
        {
            var audio = WaveEnumerableAudioContainerBuilder.Build(new AudioFormat(44100, 16, 1, AudioFormatType.Pcm), lengthInSeconds: 10);
            var sampler = new FromMonoAudioSampler(2);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            var newData = await newAudio.GetAudioSamplesAsync().ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(audio.DataSubChunk.ChunkSize*2, newAudio.DataSubChunk.ChunkSize);
            Assert.AreEqual(newAudio.DataSubChunk.ChunkSize, newData.Sum(x => x.Definition.Bits / 8));
        }

        [Test]
        public async Task ShouldHaveSameLengthAfterSampling()
        {
            var audio = WaveEnumerableAudioContainerBuilder.Build(new AudioFormat(44100, 16, 1, AudioFormatType.Pcm), lengthInSeconds: 10);
            var sampler = new FromMonoAudioSampler(2);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);

            Assert.AreEqual(audio.GetLength().Ticks, newAudio.GetLength().Ticks);
        }
    }
}
