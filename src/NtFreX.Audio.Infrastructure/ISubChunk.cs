namespace NtFreX.Audio.Infrastructure
{
    public interface ISubChunk
    {
        string ChunkId { get; }
        uint ChunkSize { get; }
    }
}
