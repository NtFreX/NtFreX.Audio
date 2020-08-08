using NtFreX.Audio.Infrastructure.Container;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Containers
{
    public interface IDataSubChunk: ISubChunk
    {
        IAsyncEnumerable<byte[]> GetAudioSamplesAsBufferAsync([MaybeNull] CancellationToken cancellationToken = default);
    }
}
