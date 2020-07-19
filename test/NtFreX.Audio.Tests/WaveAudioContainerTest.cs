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
            var audio = WaveContainerBuilder.Build(10, 32, 44100, RiffChunkDescriptor.ChunkIdentifierRIFF);
            
            Assert.IsTrue(audio.IsDataLittleEndian());
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void ShouldBeBigEndianWhenRiff()
        {
            var audio = WaveContainerBuilder.Build(10, 32, 44100, RiffChunkDescriptor.ChunkIdentifierRIFX);

            Assert.IsFalse(audio.IsDataLittleEndian());
        }
    }
}
