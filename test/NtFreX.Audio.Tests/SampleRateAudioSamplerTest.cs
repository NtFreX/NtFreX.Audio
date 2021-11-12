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
    public class SampleRateAudioSamplerTest
    {
        // TODO: add to the tests [Test]
        // TODO: or thow exception on multiple enumeration but that would be the poor mans choise as it should work
        public static async Task CanEnumerateSampledContainerMultipleTimes()
        {
            var audio = IntermediateAudioContainerBuilder.BuildSilence(new AudioFormat(WellKnownSampleRate.Hz22050, 32, 1, AudioFormatType.Pcm), lengthInSeconds: 10);
            var sampler = new SampleRateAudioSampler(WellKnownSampleRate.Hz44100);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            var newData = await newAudio.ToArrayAsync().ConfigureAwait(false);
            var oldData = await audio.ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(newData.Length, oldData.Length);
        }

        [TestCase((uint)2, (uint)4)]
        [TestCase((uint)4, (uint)6)]
        [TestCase((uint)3, (uint)5)]
        [TestCase((uint)4, (uint)8)]
        [TestCase(WellKnownSampleRate.Hz44100, WellKnownSampleRate.Hz48000)]
        [TestCase(WellKnownSampleRate.Hz16000, WellKnownSampleRate.Hz48000)]
        [TestCase(WellKnownSampleRate.Hz8000, WellKnownSampleRate.Hz32000)]
        [TestCase(WellKnownSampleRate.Hz32000, WellKnownSampleRate.Hz48000)]
        [TestCase((uint)8, (uint)4)]
        [TestCase(WellKnownSampleRate.Hz48000, WellKnownSampleRate.Hz44100)]
        [TestCase(WellKnownSampleRate.Hz32000, WellKnownSampleRate.Hz8000)]
        public async Task ShouldSampleCorrectByteAmount(uint fromSampleRate, uint toSampleRate)
        {
            var audio = IntermediateAudioContainerBuilder.BuildSilence(new AudioFormat(fromSampleRate, 32, 1, AudioFormatType.Pcm), lengthInSeconds: 10);
            var sampler = new SampleRateAudioSampler(toSampleRate);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            var newData = await newAudio.ToArrayAsync().ConfigureAwait(false);
            var oldDataSize = audio.GetByteLength();
            
            float factor = toSampleRate / (float)fromSampleRate;
            int expectedNewSize = (int)(audio.GetByteLength() * factor);
            var expectedNewDataSize = (uint)(oldDataSize * factor);
            var newDataSize = newData.Sum(x => x.Definition.Bytes);

            Assert.AreEqual(expectedNewDataSize, newDataSize);
            Assert.AreEqual(expectedNewDataSize, newAudio.GetByteLength());
            Assert.AreEqual(expectedNewDataSize, expectedNewSize);
        }

        [TestCase((uint)4, (uint)8)]
        [TestCase(WellKnownSampleRate.Hz44100, WellKnownSampleRate.Hz48000)]
        [TestCase((uint)8, (uint)4)]
        [TestCase(WellKnownSampleRate.Hz48000, WellKnownSampleRate.Hz44100)]
        public async Task ShouldBeSameLengthAfterSampling(uint fromSampleRate, uint toSampleRate)
        {
            var audio = IntermediateAudioContainerBuilder.BuildSilence(new AudioFormat(fromSampleRate, 32, 1, AudioFormatType.Pcm), lengthInSeconds: 10);
            var sampler = new SampleRateAudioSampler(toSampleRate);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);

            Assert.AreEqual(audio.GetLength().Ticks, newAudio.GetLength().Ticks);
        }
    }
}
