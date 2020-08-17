using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Containers
{
    public interface IDataSubChunk: ISubChunk
    {
        void SeekTo(long position);
        IAsyncEnumerable<Sample> GetAudioSamplesAsync([MaybeNull] CancellationToken cancellationToken = default);
    }
}
