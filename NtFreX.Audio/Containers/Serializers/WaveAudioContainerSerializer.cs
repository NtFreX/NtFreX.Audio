using NtFreX.Audio.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace NtFreX.Audio.Containers.Serializers
{
    internal class WaveAudioContainerSerializer : AudioContainerSerializer<WaveAudioContainer>
    {
        public override string PreferredFileExtension => "wav";

        public override byte[] ToData(WaveAudioContainer container)
        {
            return new List<byte>()
                .Concat(container.RiffChunkDescriptor.ChunkId.ToByteArray())
                .Concat(container.RiffChunkDescriptor.ChunkSize.ToByteArray())
                .Concat(container.RiffChunkDescriptor.Format.ToByteArray())
                .Concat(container.FmtSubChunk.Subchunk1Id.ToByteArray())
                .Concat(container.FmtSubChunk.Subchunk1Size.ToByteArray())
                .Concat(container.FmtSubChunk.AudioFormat.ToByteArray())
                .Concat(container.FmtSubChunk.NumChannels.ToByteArray())
                .Concat(container.FmtSubChunk.SampleRate.ToByteArray())
                .Concat(container.FmtSubChunk.ByteRate.ToByteArray())
                .Concat(container.FmtSubChunk.BlockAlign.ToByteArray())
                .Concat(container.FmtSubChunk.BitsPerSample.ToByteArray())
                .Concat(container.DataSubChunk.Subchunk2Id.ToByteArray())
                .Concat(container.DataSubChunk.Subchunk2Size.ToByteArray())
                .Concat(container.DataSubChunk.Data)
                .ToArray();
        }

        public override WaveAudioContainer FromData(byte[] data)
        {
            return new WaveAudioContainer(
               new RiffChunkDescriptor(chunkId: data.TakeInt(0), chunkSize: data.TakeInt(4), format: data.TakeInt(8)),
               new FmtSubChunk(subchunk1Id: data.TakeInt(12), subchunk1Size: data.TakeInt(16), audioFormat: data.TakeUShort(20),
                                numChannels: data.TakeUShort(22), sampleRate: data.TakeInt(24), byteRate: data.TakeUInt(28),
                                blockAlign: data.TakeUShort(32), bitsPerSample: data.TakeUShort(34)),
               new DataSubChunk(subchunk2Id: data.TakeInt(36), subchunk2Size: data.TakeInt(40), data: data.Skip(44).ToArray())
            );
        }
    }
}
