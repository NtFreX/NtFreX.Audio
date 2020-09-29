using NtFreX.Audio.Infrastructure.Container;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Containers
{
    public class UnknownSubChunk : ISubChunk<UnknownSubChunk>
    {
        public string ChunkId { get; }

        public uint ChunkSize { get; }

        public IReadOnlyList<byte> SubchunkData { get; }

        public UnknownSubChunk([NotNull] string subchunkId, uint subchunkSize, [NotNull] IReadOnlyList<byte> subchunkData)
        {
            ChunkId = subchunkId;
            ChunkSize = subchunkSize;
            SubchunkData = subchunkData;
        }

        public UnknownSubChunk WithChunkId(string chunkId) => new UnknownSubChunk(chunkId, ChunkSize, SubchunkData);
        public UnknownSubChunk WithChunkSize(uint chunkSize) => new UnknownSubChunk(ChunkId, chunkSize, SubchunkData);
    }
}
