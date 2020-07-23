namespace NtFreX.Audio.Containers
{
    public interface ISubChunk
    {
        string ChunkId { get; }
        uint ChunkSize { get; }
    }
}
