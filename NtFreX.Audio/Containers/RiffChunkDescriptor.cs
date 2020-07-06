using System;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Containers
{
    public class RiffChunkDescriptor
    {
        public const string RIFF = nameof(RIFF);
        public const string WAVE = nameof(WAVE);

        /// <summary>
        /// Contains the letters "RIFF" in ASCII form (0x52494646 big-endian form).
        /// </summary>
        public string ChunkId { [return:NotNull] get; }

        /// <summary>
        /// This is the size of the entire file in bytes minus 8 bytes for the two fields not included in this count: ChunkID and ChunkSize.
        /// </summary>
        public uint ChunkSize { get; }

        /// <summary>
        /// Contains the letters "WAVE" (0x57415645 big-endian form).
        /// </summary>
        public string Format { [return: NotNull] get; }

        [return: NotNull] public RiffChunkDescriptor WithChunkId([NotNull] string chunkId) => new RiffChunkDescriptor(chunkId, ChunkSize, Format);
        [return: NotNull] public RiffChunkDescriptor WithChunkSize(uint chunkSize) => new RiffChunkDescriptor(ChunkId, chunkSize, Format);
        [return: NotNull] public RiffChunkDescriptor WithFormat([NotNull] string format) => new RiffChunkDescriptor(ChunkId, ChunkSize, format);

        public RiffChunkDescriptor([NotNull] string chunkId, uint chunkSize, [NotNull] string format)
        {
            ChunkId = chunkId;
            ChunkSize = chunkSize;
            Format = format;

            ThrowIfInvalid();
        }

        private void ThrowIfInvalid()
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            if (ChunkId != RIFF)
            {
                // TODO: support RIFX => The default byte ordering assumed for WAVE data files is little-endian. Files written using the big-endian byte ordering scheme have the identifier RIFX instead of RIFF.
                throw new ArgumentException("The value has to contain the letters 'RIFF' (0x52494646 big-endian form)", nameof(ChunkId));
            }

            if (Format != WAVE)
            {
                throw new ArgumentException("The value has to contain the letters 'WAVE' (0x57415645 big-endian form)", nameof(ChunkId));
            }
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
        }
    }
}
