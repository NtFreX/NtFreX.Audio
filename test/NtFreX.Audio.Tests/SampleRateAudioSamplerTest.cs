using Dasync.Collections;
using NtFreX.Audio.Containers;
using NtFreX.Audio.Math;
using NtFreX.Audio.Samplers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtFreX.Audio.Tests
{
    public static class SampleRate
    {
        public const int Hz8000 = 8000;
        public const int Hz11025 = 11025;
        public const int Hz16000 = 16000;
        public const int Hz22050 = 22050;
        public const int Hz32000 = 32000;
        public const int Hz37800 = 37800;
        public const int Hz44056 = 44056;
        public const int Hz44100 = 44100;
        public const int Hz47250 = 47250;
        public const int Hz48000 = 48000;
        public const int Hz50000 = 50000;
        public const int Hz50400 = 50400;
        public const int Hz64000 = 64000;
        public const int Hz88200 = 88200;
        public const int Hz96000 = 96000;
        public const int Hz17640 = 17640;
        public const int Hz192000 = 192000;
        public const int Hz352800 = 352800;
    }

    [TestFixture]
    public class SampleRateAudioSamplerTest
    {
        [TestCase(2, 4)]
        [TestCase(4, 6)]
        [TestCase(3, 5)]
        [TestCase(4, 8)]
        [TestCase(SampleRate.Hz44100, SampleRate.Hz48000)]
        [TestCase(SampleRate.Hz8000, SampleRate.Hz32000)]
        [TestCase(8, 4)]
        [TestCase(SampleRate.Hz48000, SampleRate.Hz44100)]
        public async Task ShouldSampleCorrectByteAmount(int fromSampleRate, int toSampleRate)
        {
            var audio = WaveContainerBuilder.Build(10, 16, (uint) fromSampleRate);
            var sampler = new SampleRateAudioSampler((uint) toSampleRate);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            var newData = await newAudio.DataSubChunk.Data.ToArrayAsync().ConfigureAwait(false);

            int expectedNewSize = (int)(audio.DataSubChunk.Subchunk2Size * (toSampleRate / (float)fromSampleRate));
            Assert.AreEqual(expectedNewSize, newData.SelectMany(x => x).Count());
            Assert.AreEqual(expectedNewSize, newAudio.DataSubChunk.Subchunk2Size);
        }

        [TestCase(4, 8)]
        [TestCase(SampleRate.Hz44100, SampleRate.Hz48000)]
        [TestCase(8, 4)]
        [TestCase(SampleRate.Hz48000, SampleRate.Hz44100)]
        public async Task ShouldBeSameLengthAfterSampling(int fromSampleRate, int toSampleRate)
        {
            var audio = WaveContainerBuilder.Build(10, 32, (uint)fromSampleRate);
            var sampler = new SampleRateAudioSampler((uint)toSampleRate);

            var newAudio = await sampler.SampleAsync(audio).ConfigureAwait(false);
            var newData = (await newAudio.DataSubChunk.Data.ToArrayAsync().ConfigureAwait(false)).SelectMany(x => x);

            Assert.AreEqual(audio.GetLength().Ticks, newAudio.GetLength().Ticks);
        }
    }
}
