using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Containers
{
    public sealed class EnumerableDataSubChunk : DataSubChunk<EnumerableDataSubChunk>
    {
        /// <summary>
        /// The actual sound data.
        /// </summary>
        public IAsyncEnumerable<byte[]> Data { get; }

        public EnumerableDataSubChunk([NotNull] string chunkId, uint chunkSize, [NotNull] IAsyncEnumerable<Sample> data)
            : this(chunkId, chunkSize, data.SelectAsync(x => x.AsByteArray())) { }

        public EnumerableDataSubChunk([NotNull] string chunkId, uint chunkSize, [NotNull] IAsyncEnumerable<byte[]> data)
            : base(chunkId, chunkSize)
        {
            Data = data;
        }

        [return: NotNull] public override EnumerableDataSubChunk WithChunkId([NotNull] string chunkId) => new EnumerableDataSubChunk(chunkId, ChunkSize, Data);
        [return: NotNull] public override EnumerableDataSubChunk WithChunkSize(uint chunkSize) => new EnumerableDataSubChunk(ChunkId, chunkSize, Data);
        [return: NotNull] public EnumerableDataSubChunk WithData([NotNull] IAsyncEnumerable<Sample> data) => new EnumerableDataSubChunk(ChunkId, ChunkSize, data);

        [return: NotNull]
        public override IAsyncEnumerable<byte[]> GetAudioSamplesAsBufferAsync([MaybeNull] CancellationToken cancellationToken = default)
            => Data;
    }
}
