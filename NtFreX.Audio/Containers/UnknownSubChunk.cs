using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Containers
{
    public class UnknownSubChunk
    {
        public string SubchunkId { [return:NotNull] get; }

        public uint SubchunkSize { get; }

        public byte[] SubchunkData { get; }

        public UnknownSubChunk([NotNull] string subchunkId, uint subchunkSize, byte[] subchunkData)
        {
            SubchunkId = subchunkId;
            SubchunkSize = subchunkSize;
            SubchunkData = subchunkData;
        }
    }
}
