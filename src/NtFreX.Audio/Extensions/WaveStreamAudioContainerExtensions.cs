using NtFreX.Audio.Containers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Extensions
{
    public static class WaveStreamAudioContainerExtensions
    {
        [return: NotNull]
        public static WaveEnumerableAudioContainer ToEnumerable([NotNull] this WaveStreamAudioContainer container, CancellationToken cancellationToken = default)
        {
            _ = container ?? throw new ArgumentNullException(nameof(container));

            var data = new EnumerableDataSubChunk(container.DataSubChunk.ChunkId, container.DataSubChunk.ChunkSize, container.GetAudioSamplesAsync(cancellationToken));
            return new WaveEnumerableAudioContainer(container.RiffSubChunk, container.FmtSubChunk, data, container.UnknownSubChunks);
        }
    }
}
