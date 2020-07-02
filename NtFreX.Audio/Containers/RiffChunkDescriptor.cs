using System;

namespace NtFreX.Audio.Containers
{
    public class RiffChunkDescriptor
    {
        public const string RIFF = "52-49-46-46";

        /// <summary>
        /// Contains the letters "RIFF" in ASCII form (0x52494646 big-endian form).
        /// </summary>
        public int ChunkId { get; }

        /// <summary>
        /// This is the size of the entire file in bytes minus 8 bytes for the two fields not included in this count: ChunkID and ChunkSize.
        /// </summary>
        public int ChunkSize { get; }

        /// <summary>
        /// Contains the letters "WAVE" (0x57415645 big-endian form).
        /// </summary>
        public int Format { get; }

        public RiffChunkDescriptor WithChunkId(int chunkId) => new RiffChunkDescriptor(chunkId, ChunkSize, Format);
        public RiffChunkDescriptor WithChunkSize(int chunkSize) => new RiffChunkDescriptor(ChunkId, chunkSize, Format);
        public RiffChunkDescriptor WithFormat(int format) => new RiffChunkDescriptor(ChunkId, ChunkSize, format);

        public RiffChunkDescriptor(int chunkId, int chunkSize, int format)
        {
            ChunkId = chunkId;
            ChunkSize = chunkSize;
            Format = format;

            ThrowIfInvalid();
        }

        private void ThrowIfInvalid()
        {
            if (GetChunkId() != RIFF)
            {
                // TODO: support RIFX => The default byte ordering assumed for WAVE data files is little-endian. Files written using the big-endian byte ordering scheme have the identifier RIFX instead of RIFF.
                throw new ArgumentException("The value has to contain the letters 'RIFF' (0x52494646 big-endian form)", nameof(ChunkId));
            }

            if (BitConverter.ToString(BitConverter.GetBytes(Format)) != "57-41-56-45")
            {
                throw new ArgumentException("The value has to contain the letters 'WAVE' (0x57415645 big-endian form)", nameof(ChunkId));
            }
        }

        public string GetChunkId() => BitConverter.ToString(BitConverter.GetBytes(ChunkId));
    }
}
