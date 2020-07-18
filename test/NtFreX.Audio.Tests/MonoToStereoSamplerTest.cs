using Dasync.Collections;
using NtFreX.Audio.Containers;
using NtFreX.Audio.Samplers;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Tests
{
    [TestFixture]
    public class MonoToStereoSamplerTest {
        [Test]
        public async Task ShouldHaveTwoChannels()
        {
            var audio = WaveContainerBuilder.Build(10, 16, 44100);
            var sampler = new MonoToStereoAudioSampler();

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            
            Assert.AreEqual(2, newAudio.FmtSubChunk.NumChannels);
        }
        [Test]
        public async Task ShouldHaveDoubleAmountOfData()
        {
            var audio = WaveContainerBuilder.Build(10, 16, 44100);
            var sampler = new MonoToStereoAudioSampler();

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            var newData = await newAudio.DataSubChunk.Data.ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(audio.DataSubChunk.Subchunk2Size*2, newAudio.DataSubChunk.Subchunk2Size);
            Assert.AreEqual(newAudio.DataSubChunk.Subchunk2Size, newData.SelectMany(x => x).Count());
        }

        [Test]
        public async Task ShouldHaveSameLengthAfterSampling()
        {
            var audio = WaveContainerBuilder.Build(10, 16, 44100);
            var sampler = new MonoToStereoAudioSampler();

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);

            Assert.AreEqual(audio.GetLength().Ticks, newAudio.GetLength().Ticks);
        }
    }
}
