using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using NtFreX.Audio.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers
{
    // TODO: make ref strcut and use span propery?
    public sealed class DataSubChunk : ISubChunk<DataSubChunk>, ISubChunk, ISeekableAsyncEnumerable<IReadOnlyList<byte>>
    {
        public const string ChunkIdentifer = "data";
        public const int ChunkHeaderSize = 8;

        /// <summary>
        /// Contains the letters "data" (0x64617461 big-endian form).
        /// </summary>
        public string ChunkId { get; }

        /// <summary>
        /// == NumSamples * NumChannels * BitsPerSample/8
        /// </summary>
        public uint ChunkSize { get; }

        private readonly ISeekableAsyncEnumerable<IReadOnlyList<byte>> data;

        public DataSubChunk(long startIndex, string chunkId, uint chunkSize, Stream data)
            : this(chunkId, data.ToEnumerable(startIndex + ChunkHeaderSize, chunkSize + startIndex + ChunkHeaderSize), chunkSize) { }

        public DataSubChunk(string chunkId, ISeekableAsyncEnumerable<IReadOnlyList<byte>> data, uint size)
        {
            _ = data ?? throw new ArgumentNullException(nameof(data));

            ChunkId = chunkId;
            ChunkSize = size;

            this.data = data;

            ThrowIfInvalid();
        }

        public DataSubChunk WithChunkId(string chunkId)
            => throw new NotSupportedException();
        public DataSubChunk WithChunkSize(uint chunkSize)
            => throw new NotSupportedException();

        public ValueTask DisposeAsync()
            => data.DisposeAsync();
        public long GetDataLength()
            => data.GetDataLength();

        public ISeekableAsyncEnumerator<IReadOnlyList<byte>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => data.GetAsyncEnumerator(cancellationToken);

        private void ThrowIfInvalid()
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            if (ChunkId != ChunkIdentifer)
            {
                throw new ArgumentException(ExceptionMessages.DataSubChunkIdMissmatch, nameof(ChunkId));
            }
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
        }
    }
}
