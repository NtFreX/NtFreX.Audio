using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Infrastructure.Container
{
    public interface IRiffSubChunk : ISubChunk
    {
        string Format { get; }

        IRiffSubChunk WithChunkId([NotNull] string chunkId);
        IRiffSubChunk WithChunkSize(uint chunkSize);
        IRiffSubChunk WithFormat([NotNull] string format);

        bool IsDataLittleEndian();
    }
}
