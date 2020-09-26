using NtFreX.Audio.Containers;
using NtFreX.Audio.Containers.Serializers;
using NtFreX.Audio.Containers.Wave;
using NtFreX.Audio.Devices;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Infrastructure;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NtFreX.Audio.Tests
{
    [TestFixture]
    public class StreamWaveAudioSinkTest
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        public async Task ShouldUpdateRiffChunkSizeWhenFinishingAsync(int count)
        {
            // arrange
            var stream = new MemoryStream();
            var format = new AudioFormat(WellKnownSampleRate.Hz44100, 32, 1, AudioFormatType.Pcm);
            var sink = await StreamWaveAudioSink.CreateAsync(stream, format).ConfigureAwait(false);
            
            // act
            var size = 100;
            var data = new byte[size];

            for (var i = 0; i < count; i++)
            {
                sink.DataReceived(data);
            }

            await sink.DisposeAsync().ConfigureAwait(false);

            // assert
            stream.Seek(0, SeekOrigin.Begin);

            var serializer = new WaveAudioContainerSerializer();
            var audio = await serializer.FromStreamAsync(stream).ConfigureAwait(false);
            var expectedTotalSize = (uint)((size * count) + WaveAudioContainer.DefaultHeaderSize);

            Assert.AreEqual(expectedTotalSize, audio.RiffSubChunk.ChunkSize);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        public async Task ShouldUpdateDataChunkSizeWhenFinishingAsync(int count)
        {
            // arrange
            var stream = new MemoryStream();
            var format = new AudioFormat(WellKnownSampleRate.Hz44100, 32, 1, AudioFormatType.Pcm);
            var sink = await StreamWaveAudioSink.CreateAsync(stream, format).ConfigureAwait(false);

            // act
            var size = 100;
            var data = new byte[size];

            for (var i = 0; i < count; i++)
            {
                sink.DataReceived(data);
            }

            await sink.DisposeAsync().ConfigureAwait(false);

            // assert
            stream.Seek(0, SeekOrigin.Begin);

            var serializer = new WaveAudioContainerSerializer();
            var audio = await serializer.FromStreamAsync(stream).ConfigureAwait(false);
            var expectedSize = (uint) (size * count);

            Assert.AreEqual(expectedSize, audio.DataSubChunk.ChunkSize);
        }
    }
}
