using NtFreX.Audio.Containers;
using NUnit.Framework;

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
        public void ShouldBeBigEndianWhenRifX()
        {
            var audio = TestHelper.Build(10, 32, 44100, riffChunkId: RiffChunkDescriptor.ChunkIdentifierRIFX);

            Assert.IsFalse(audio.IsDataLittleEndian());
        }
    }
}
