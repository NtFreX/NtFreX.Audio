using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Containers
{
    public class UnknownSubChunk
    {
        public string SubchunkId { get; }

        public uint SubchunkSize { get; }

        public IReadOnlyList<byte> SubchunkData { get; }

        public UnknownSubChunk([NotNull] string subchunkId, uint subchunkSize, [NotNull] IReadOnlyList<byte> subchunkData)
        {
            SubchunkId = subchunkId;
            SubchunkSize = subchunkSize;
            SubchunkData = subchunkData;
        }
    }
}
