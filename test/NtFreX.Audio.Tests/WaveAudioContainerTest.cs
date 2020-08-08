using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Math;
using NUnit.Framework;

namespace NtFreX.Audio.Tests
{
    [TestFixture]
    public class WaveAudioContainerTest
    {
        [Test]
        public void ShouldBeLittleEndianWhenRiff()
        {
            var isLittleEndian = true;
            var format = new AudioFormat(44100, 32, 1, AudioFormatType.Pcm);
            var audio = WaveEnumerableAudioContainerBuilder.Build(format, lengthInSeconds: 10, isLittleEndian);

            Assert.AreEqual(RiffSubChunk.ChunkIdentifierRIFF, audio.RiffSubChunk.ChunkId);
            Assert.IsTrue(audio.IsDataLittleEndian());
        }

        [Test]
        public void ShouldBeBigEndianWhenRifX()
        {
            var isLittleEndian = false;
            var format = new AudioFormat(44100, 32, 1, AudioFormatType.Pcm);
            var audio = WaveEnumerableAudioContainerBuilder.Build(format, lengthInSeconds: 10, isLittleEndian);

            Assert.AreEqual(RiffSubChunk.ChunkIdentifierRIFX, audio.RiffSubChunk.ChunkId);
            Assert.IsFalse(audio.IsDataLittleEndian());
        }
    }
}
