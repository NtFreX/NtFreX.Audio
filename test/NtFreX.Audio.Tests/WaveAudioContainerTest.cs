using NtFreX.Audio.Containers;
using NUnit.Framework;
using System;

namespace NtFreX.Audio.Tests
{
    [TestFixture]
    public class WaveAudioContainerTest
    {
        [Test]
        public void ShouldBeLittleEndianWhenRiff()
        {
            var audio = TestHelper.Build(10, 32, 44100, riffChunkId: RiffChunkDescriptor.ChunkIdentifierRIFF);
            
            Assert.IsTrue(audio.IsDataLittleEndian());
        }

        [Test]
        public void ShouldBeBigEndianWhenRiffOrNotSupportedYet()
        {
            Assert.Throws<ArgumentException>(() => TestHelper.Build(10, 32, 44100, riffChunkId: RiffChunkDescriptor.ChunkIdentifierRIFX));
        }
    }
}
