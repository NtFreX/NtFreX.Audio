using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using NtFreX.Audio.Samplers;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace NtFreX.Audio.Tests
{
    [TestFixture]
    public class MonoToStereoSamplerTest 
    {
        [Test]
        public async Task ShouldHaveTwoChannels()
        {
            var audio = IntermediateAudioContainerBuilder.BuildSilence(new AudioFormat(44100, 16, 1, AudioFormatType.Pcm), lengthInSeconds: 10);
            var sampler = new FromMonoAudioSampler(2);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            
            Assert.AreEqual(2, newAudio.GetFormat().Channels);
        }
        [Test]
        public async Task ShouldHaveDoubleAmountOfData()
        {
            var audio = IntermediateAudioContainerBuilder.BuildSilence(new AudioFormat(44100, 16, 1, AudioFormatType.Pcm), lengthInSeconds: 10);
            var sampler = new FromMonoAudioSampler(2);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            var newData = await newAudio.ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(audio.GetDataLength()*2, newAudio.GetDataLength());
            Assert.AreEqual(newAudio.GetByteLength(), newData.Sum(x => x.Definition.Bytes));
        }

        [Test]
        public async Task ShouldHaveSameLengthAfterSampling()
        {
            var audio = IntermediateAudioContainerBuilder.BuildSilence(new AudioFormat(44100, 16, 1, AudioFormatType.Pcm), lengthInSeconds: 10);
            var sampler = new FromMonoAudioSampler(2);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);

            Assert.AreEqual(audio.GetLength().Ticks, newAudio.GetLength().Ticks);
        }
    }
}
