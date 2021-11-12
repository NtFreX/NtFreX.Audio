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

        // TODO: create demo with this code
        //public static IntermediateAudioContainer EternalSilenceContainer()
        //{
        //    var format = new AudioFormat(WellKnownSampleRate.Hz44100, 8, 1, AudioFormatType.Pcm);
        //    return IntermediateAudioContainerBuilder.Build(
        //        format, 
        //        EternalSilence(format, isLittleEndian: true)
        //            .ToAsyncEnumerable()
        //            .ToNonSeekable(), 
        //        realByteLength: null);
        //}

        //public static IEnumerable<Memory<byte>> EternalSilence(IAudioFormat format, bool isLittleEndian)
        //{
        //    if(format == null)
        //    {
        //        throw new ArgumentNullException(nameof(format));
        //    }

        //    while (true)
        //    {
        //        yield return new Sample(0, new SampleDefinition(format.Type, format.BitsPerSample, isLittleEndian)).AsByteArray();
        //    }    
        //}
    }
}
