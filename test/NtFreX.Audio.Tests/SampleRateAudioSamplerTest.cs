using Dasync.Collections;
using NtFreX.Audio.Containers;
using NtFreX.Audio.Samplers;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace NtFreX.Audio.Tests
{
    [TestFixture]
    public class SampleRateAudioSamplerTest
    {
        [TestCase(2, 4)]
        [TestCase(4, 6)]
        [TestCase(3, 5)]
        [TestCase(4, 8)]
        [TestCase(WellKnownSampleRate.Hz44100, WellKnownSampleRate.Hz48000)]
        [TestCase(WellKnownSampleRate.Hz16000, WellKnownSampleRate.Hz48000)]
        [TestCase(WellKnownSampleRate.Hz8000, WellKnownSampleRate.Hz32000)]
        [TestCase(WellKnownSampleRate.Hz32000, WellKnownSampleRate.Hz48000)]
        [TestCase(8, 4)]
        [TestCase(WellKnownSampleRate.Hz48000, WellKnownSampleRate.Hz44100)]
        [TestCase(WellKnownSampleRate.Hz32000, WellKnownSampleRate.Hz8000)]
        public async Task ShouldSampleCorrectByteAmount(int fromSampleRate, int toSampleRate)
        {
            var audio = WaveContainerBuilder.Build(10, 32, (uint) fromSampleRate);
            var sampler = new SampleRateAudioSampler((uint) toSampleRate);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            var oldData = await audio.DataSubChunk.Data.ToArrayAsync().ConfigureAwait(false);
            var newData = await newAudio.DataSubChunk.Data.ToArrayAsync().ConfigureAwait(false);

            float factor = toSampleRate / (float)fromSampleRate;
            int expectedNewSize = (int)(audio.DataSubChunk.ChunkSize * factor);
            var expectedNewDataSize = (uint)(oldData.SelectMany(x => x).Count() * factor);
            var newDataSize = newData.SelectMany(x => x).Count();

            Assert.AreEqual(expectedNewDataSize, newDataSize);
            Assert.AreEqual(expectedNewDataSize, newAudio.DataSubChunk.ChunkSize);
            Assert.AreEqual(expectedNewDataSize, expectedNewSize);
        }

        [TestCase(4, 8)]
        [TestCase(WellKnownSampleRate.Hz44100, WellKnownSampleRate.Hz48000)]
        [TestCase(8, 4)]
        [TestCase(WellKnownSampleRate.Hz48000, WellKnownSampleRate.Hz44100)]
        public async Task ShouldBeSameLengthAfterSampling(int fromSampleRate, int toSampleRate)
        {
            var audio = WaveContainerBuilder.Build(10, 32, (uint)fromSampleRate);
            var sampler = new SampleRateAudioSampler((uint)toSampleRate);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);

            Assert.AreEqual(audio.GetLength().Ticks, newAudio.GetLength().Ticks);
        }
    }
}
