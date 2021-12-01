using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using NtFreX.Audio.Math;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NtFreX.Audio.Tests
{
    [TestFixture]
    public class WaveBuilderTest
    {
        [Test]
        public async Task SeekableLengthOfSilenceShouldBeCorrectAsync()
        {
            var lengthInSeconds = 10;
            var format = new AudioFormat(WellKnownSampleRate.Hz44100, 64, 2, AudioFormatType.Pcm) as IAudioFormat;
            var silence = WaveBuilder.Silence(format, 10);
            var data = await silence.ToArrayAsync().ConfigureAwait(false);

            Assert.AreEqual(format.SampleRate * format.BytesPerSample * format.Channels * lengthInSeconds, data.Length);
        }
    }
}
