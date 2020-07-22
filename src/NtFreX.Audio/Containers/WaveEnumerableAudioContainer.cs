using NtFreX.Audio.Containers;
using NtFreX.Audio.Containers.Serializers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers
{
    public class WaveEnumerableAudioContainer : WaveAudioContainer<EnumerableDataSubChunk>
    {
        [return: NotNull] internal WaveEnumerableAudioContainer WithRiffChunkDescriptor([NotNull] Func<RiffChunkDescriptor, RiffChunkDescriptor> riffChunkDescriptor) => new WaveEnumerableAudioContainer(riffChunkDescriptor(RiffChunkDescriptor), FmtSubChunk, DataSubChunk, UnknownSubChuncks);
        [return: NotNull] internal WaveEnumerableAudioContainer WithFmtSubChunk([NotNull] Func<FmtSubChunk, FmtSubChunk> fmtSubChunk) => new WaveEnumerableAudioContainer(RiffChunkDescriptor, fmtSubChunk(FmtSubChunk), DataSubChunk, UnknownSubChuncks);
        [return: NotNull] internal WaveEnumerableAudioContainer WithDataSubChunk([NotNull] Func<EnumerableDataSubChunk, EnumerableDataSubChunk> dataSubChunk) => new WaveEnumerableAudioContainer(RiffChunkDescriptor, FmtSubChunk, dataSubChunk(DataSubChunk), UnknownSubChuncks);
        [return: NotNull] internal WaveEnumerableAudioContainer WithRiffSubChunks([NotNull] UnknownSubChunk[] riffSubChunks) => new WaveEnumerableAudioContainer(RiffChunkDescriptor, FmtSubChunk, DataSubChunk, riffSubChunks);

        public WaveEnumerableAudioContainer([NotNull] RiffChunkDescriptor riffChunkDescriptor, [NotNull] FmtSubChunk fmtSubChunk, [NotNull] EnumerableDataSubChunk dataSubChunk, [NotNull] IReadOnlyList<UnknownSubChunk> riffSubChuncks)
            : base(riffChunkDescriptor, fmtSubChunk, dataSubChunk, riffSubChuncks) { }

        [return: NotNull]
        public Task<WaveStreamAudioContainer> ToFileAsync([NotNull] string path, [NotNull] FileMode fileMode = FileMode.CreateNew, [MaybeNull] CancellationToken cancellationToken = default)
            => ToStreamAsync(new FileStream(path, fileMode, FileAccess.ReadWrite), cancellationToken);

        [return: NotNull]
        public async Task<WaveStreamAudioContainer> ToStreamAsync([NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            var length = await WaveAudioContainerSerializer.WriteHeadersAsync(RiffChunkDescriptor, FmtSubChunk, UnknownSubChuncks, DataSubChunk, stream, cancellationToken).ConfigureAwait(false);
            await WaveAudioContainerSerializer.WriteDataAsync(GetAudioSamplesAsync(cancellationToken), stream, cancellationToken).ConfigureAwait(false);

#pragma warning disable CA2000 // Dispose objects before losing scope => The method that raised the warning returns an IDisposable object that wraps your object
            return new WaveStreamAudioContainer(RiffChunkDescriptor, FmtSubChunk, new StreamDataSubChunk(length - Containers.DataSubChunk.ChunkHeaderSize, DataSubChunk.ChunkId, DataSubChunk.ChunkSize, stream), UnknownSubChuncks);
#pragma warning restore CA2000 // Dispose objects before losing scope
        }
    }
}
