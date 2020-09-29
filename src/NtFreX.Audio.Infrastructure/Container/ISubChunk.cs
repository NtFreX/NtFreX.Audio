namespace NtFreX.Audio.Infrastructure.Container
{
    public interface ISubChunk
    {
        string ChunkId { get; }
        uint ChunkSize { get; }
    }

    public interface ISubChunk<T> : ISubChunk
        where T: ISubChunk
    {
        T WithChunkId(string chunkId);
        T WithChunkSize(uint chunkSize);
    }
}
