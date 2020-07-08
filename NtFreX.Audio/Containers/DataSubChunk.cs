using NtFreX.Audio.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Containers
{
    public abstract class DataSubChunk
    {
        public const string ChunkIdentifer = "data";
        public const int ChunkHeaderSize = 8;

        /// <summary>
        /// Contains the letters "data" (0x64617461 big-endian form).
        /// </summary>
        public string Subchunk2Id { get; }

        /// <summary>
        /// == NumSamples * NumChannels * BitsPerSample/8
        /// </summary>
        public uint Subchunk2Size { get; }

        protected DataSubChunk([NotNull] string subchunk2Id, uint subchunk2Size)
        {
            Subchunk2Id = subchunk2Id;
            Subchunk2Size = subchunk2Size;

            ThrowIfInvalid();
        }

        [return: NotNull]
        public abstract IAsyncEnumerable<byte[]> GetAudioSamplesAsBufferAsync([MaybeNull] CancellationToken cancellationToken = default);

        private void ThrowIfInvalid()
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            if (Subchunk2Id != ChunkIdentifer)
            {
                throw new ArgumentException(ExceptionMessages.DataSubChunkIdMissmatch, nameof(Subchunk2Id));
            }
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
        }
    }
}
