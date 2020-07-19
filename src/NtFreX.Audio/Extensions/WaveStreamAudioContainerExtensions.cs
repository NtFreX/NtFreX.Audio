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

            var data = new EnumerableDataSubChunk(container.DataSubChunk.Subchunk2Id, container.DataSubChunk.Subchunk2Size, container.GetAudioSamplesAsync(cancellationToken));
            return new WaveEnumerableAudioContainer(container.RiffChunkDescriptor, container.FmtSubChunk, data, container.UnknownSubChuncks);
        }
    }
}
