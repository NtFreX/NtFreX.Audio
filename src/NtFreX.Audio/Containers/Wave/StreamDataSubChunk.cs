using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Containers
{
    /// <summary>
    /// Wrapper for the real stream sub chunk to allow defered creation of SampleDefinition, should be deleted when an intermediate container is built
    /// </summary>
    public sealed class StreamDataSubChunk : DataSubChunk<StreamDataSubChunk>, IDisposable
    {
        private readonly StreamBufferSubChunk streamBufferSubChunk;
        private readonly SampleDefinition definition;

        public StreamDataSubChunk(StreamBufferSubChunk streamBufferSubChunk, SampleDefinition definition)
            : base(streamBufferSubChunk?.ChunkId ?? throw new ArgumentNullException(nameof(streamBufferSubChunk)), streamBufferSubChunk.ChunkSize)
        {
            this.streamBufferSubChunk = streamBufferSubChunk;
            this.definition = definition;
        }

        public void Dispose()
        {
            streamBufferSubChunk.Dispose();
        }

        [return: NotNull]
        public override IAsyncEnumerable<Sample> GetAudioSamplesAsync([MaybeNull] CancellationToken cancellationToken = default)
            => streamBufferSubChunk.GetAudioSamplesAsBufferAsync(cancellationToken).ToAudioSamplesAsync(definition, cancellationToken);

        public override void SeekTo(long position)
            => streamBufferSubChunk.SeekTo(position);

        [return: NotNull] public override StreamDataSubChunk WithChunkId([NotNull] string chunkId) => throw new Exception("Stream containers are read only");
        [return: NotNull] public override StreamDataSubChunk WithChunkSize(uint chunkSize) => throw new Exception("Stream containers are read only");
    }
}
