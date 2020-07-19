using NtFreX.Audio.Infrastructure;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Containers
{
    public class UnknownSubChunk : ISubChunk
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
    }
}
