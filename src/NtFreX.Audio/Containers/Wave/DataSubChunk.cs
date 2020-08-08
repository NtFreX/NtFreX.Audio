using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Containers
{
    public abstract class DataSubChunk<T> : ISubChunk<T>, IDataSubChunk
        where T: ISubChunk
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

        protected DataSubChunk([NotNull] string chunkId, uint chunkSize)
        {
            ChunkId = chunkId;
            ChunkSize = chunkSize;

            ThrowIfInvalid();
        }

        [return: NotNull] public abstract T WithChunkId([NotNull] string chunkId);
        [return: NotNull] public abstract T WithChunkSize(uint chunkSize); 
        [return: NotNull] public abstract IAsyncEnumerable<byte[]> GetAudioSamplesAsBufferAsync([MaybeNull] CancellationToken cancellationToken = default);

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
