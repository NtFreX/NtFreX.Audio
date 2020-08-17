using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NtFreX.Audio.Containers
{
    public sealed class EnumerableDataSubChunk : DataSubChunk<EnumerableDataSubChunk>
    {
        /// <summary>
        /// The actual sound data.
        /// </summary>
        public IAsyncEnumerable<Sample> Data { get; }

        public EnumerableDataSubChunk([NotNull] string chunkId, uint chunkSize, [NotNull] IAsyncEnumerable<Sample> data)
            : base(chunkId, chunkSize) 
        {
            Data = data;
        }

        [return: NotNull] public override EnumerableDataSubChunk WithChunkId([NotNull] string chunkId) => new EnumerableDataSubChunk(chunkId, ChunkSize, Data);
        [return: NotNull] public override EnumerableDataSubChunk WithChunkSize(uint chunkSize) => new EnumerableDataSubChunk(ChunkId, chunkSize, Data);
        [return: NotNull] public EnumerableDataSubChunk WithData([NotNull] IAsyncEnumerable<Sample> data) => new EnumerableDataSubChunk(ChunkId, ChunkSize, data);

        [return: NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override IAsyncEnumerable<Sample> GetAudioSamplesAsync([MaybeNull] CancellationToken cancellationToken = default) => Data;

        public override void SeekTo(long position) => throw new NotImplementedException("The given data is not seekable");
    }
}
