using System.Collections.Generic;

namespace NtFreX.Audio.Infrastructure.Container
{
    public interface IRiffContainer
    {
        IRiffSubChunk RiffSubChunk { get; }
        IReadOnlyList<ISubChunk> SubChunks { get; }
    }
}
