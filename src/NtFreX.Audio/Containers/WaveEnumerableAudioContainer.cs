using NtFreX.Audio.Containers.Serializers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
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
        [return: NotNull] internal WaveEnumerableAudioContainer WithRiffSubChunk([NotNull] Func<IRiffSubChunk, IRiffSubChunk> riffSubChunk) => new WaveEnumerableAudioContainer(riffSubChunk(RiffSubChunk), FmtSubChunk, DataSubChunk, UnknownSubChunks);
        [return: NotNull] internal WaveEnumerableAudioContainer WithFmtSubChunk([NotNull] Func<FmtSubChunk, FmtSubChunk> fmtSubChunk) => new WaveEnumerableAudioContainer(RiffSubChunk, fmtSubChunk(FmtSubChunk), DataSubChunk, UnknownSubChunks);
        [return: NotNull] internal WaveEnumerableAudioContainer WithDataSubChunk([NotNull] Func<EnumerableDataSubChunk, EnumerableDataSubChunk> dataSubChunk) => new WaveEnumerableAudioContainer(RiffSubChunk, FmtSubChunk, dataSubChunk(DataSubChunk), UnknownSubChunks);
        [return: NotNull] internal WaveEnumerableAudioContainer WithRiffSubChunks([NotNull] UnknownSubChunk[] riffSubChunks) => new WaveEnumerableAudioContainer(RiffSubChunk, FmtSubChunk, DataSubChunk, riffSubChunks);

        public WaveEnumerableAudioContainer([NotNull] IRiffSubChunk riffChunkDescriptor, [NotNull] FmtSubChunk fmtSubChunk, [NotNull] EnumerableDataSubChunk dataSubChunk, [NotNull] IReadOnlyList<UnknownSubChunk> riffSubChuncks)
            : base(riffChunkDescriptor, fmtSubChunk, dataSubChunk, riffSubChuncks) { }

        [return: NotNull]
        public Task<WaveStreamAudioContainer> ToFileAsync([NotNull] string path, [NotNull] FileMode fileMode = FileMode.CreateNew, [MaybeNull] CancellationToken cancellationToken = default)
            => ToStreamAsync(new FileStream(path, fileMode, FileAccess.ReadWrite), cancellationToken);

        [return: NotNull]
        public async Task<WaveStreamAudioContainer> ToStreamAsync([NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            var length = await WaveAudioContainerSerializer.WriteHeadersAsync(RiffSubChunk, FmtSubChunk, UnknownSubChunks, DataSubChunk, stream, cancellationToken).ConfigureAwait(false);
            await WaveAudioContainerSerializer.WriteDataAsync(GetAudioSamplesAsync(cancellationToken), stream, cancellationToken).ConfigureAwait(false);

            var sampleDefinition = new SampleDefinition(Format.Type, Format.BitsPerSample, IsDataLittleEndian());
#pragma warning disable CA2000 // Dispose objects before losing scope => The method that raised the warning returns an IDisposable object that wraps your object
            var streamData = new StreamBufferSubChunk(length - DataSubChunk<ISubChunk>.ChunkHeaderSize, DataSubChunk.ChunkId, DataSubChunk.ChunkSize, stream);
            var streamWrapper = new StreamDataSubChunk(streamData, sampleDefinition);
#pragma warning restore CA2000 // Dispose objects before losing scope

            return new WaveStreamAudioContainer(RiffSubChunk, FmtSubChunk, streamWrapper, UnknownSubChunks);
        }
    }
}
