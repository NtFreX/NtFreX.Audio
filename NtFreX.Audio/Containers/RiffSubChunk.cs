namespace NtFreX.Audio.Containers
{
    public class RiffSubChunk
    {
        public int SubchunkId { get; }

        public uint SubchunkSize { get; }

        public byte[] SubchunkData { get; }

        public RiffSubChunk(int subchunkId, uint subchunkSize, byte[] subchunkData)
        {
            SubchunkId = subchunkId;
            SubchunkSize = subchunkSize;
            SubchunkData = subchunkData;
        }
    }
}
