using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Resources;
using System;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Containers
{
    public sealed class FmtSubChunk : ISubChunk<FmtSubChunk>, IAudioFormat
    {
        public const string ChunkIdentifier = "fmt ";

        /// <summary>
        /// This is the size of the rest of the Subchunk which follows the chunkSize property.
        /// </summary>
        public const int FmtChunkSize = 16;

        /// <summary>
        /// Contains the letters "fmt" (0x666d7420 big-endian form).
        /// </summary>
        public string ChunkId { get; }

        /// <summary>
        /// 16 for PCM.  This is the size of the rest of the Subchunk which follows this number.
        /// </summary>
        public uint ChunkSize { get; }

        /// <summary>
        /// PCM = 1 (i.e. Linear quantization) Values other than 1 indicate some form of compression.
        /// </summary>
        public AudioFormatType Type { get; }

        /// <summary>
        /// Mono = 1, Stereo = 2, etc.
        /// </summary>
        public ushort Channels { get; }

        /// <summary>
        /// 8000, 44100, etc.
        /// </summary>
        public uint SampleRate { get; }

        /// <summary>
        /// == SampleRate * NumChannels * BitsPerSample/8
        /// </summary>
        public uint ByteRate { get; }

        /// <summary>
        /// == NumChannels * BitsPerSample/8
        /// </summary>
        public ushort BlockAlign { get; }

        /// <summary>
        /// 8 bits = 8, 16 bits = 16, etc.
        /// </summary>
        public ushort BitsPerSample { get; }

        [return: NotNull] public FmtSubChunk WithChunkId([NotNull] string chunkId) => new FmtSubChunk(chunkId, ChunkSize, Type, Channels, SampleRate, ByteRate, BlockAlign, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithChunkSize(uint chunkSize) => new FmtSubChunk(ChunkId, chunkSize, Type, Channels, SampleRate, ByteRate, BlockAlign, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithAudioFormat(AudioFormatType type) => new FmtSubChunk(ChunkId, ChunkSize, type, Channels, SampleRate, ByteRate, BlockAlign, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithChannels(ushort channels) => new FmtSubChunk(ChunkId, ChunkSize, Type, channels, SampleRate, ByteRate, BlockAlign, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithSampleRate(uint sampleRate) => new FmtSubChunk(ChunkId, ChunkSize, Type, Channels, sampleRate, ByteRate, BlockAlign, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithByteRate(uint byteRate) => new FmtSubChunk(ChunkId, ChunkSize, Type, Channels, SampleRate, byteRate, BlockAlign, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithBlockAlign(ushort blockAlign) => new FmtSubChunk(ChunkId, ChunkSize, Type, Channels, SampleRate, ByteRate, blockAlign, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithBitsPerSample(ushort bitsPerSample) => new FmtSubChunk(ChunkId, ChunkSize, Type, Channels, SampleRate, ByteRate, BlockAlign, bitsPerSample);

        public FmtSubChunk([NotNull] string chunkId, uint chunkSize, AudioFormatType audioFormat, ushort channels, uint sampleRate, uint byteRate, ushort blockAlign, ushort bitsPerSample)
        {
            ChunkId = chunkId;
            ChunkSize = chunkSize;
            Type = audioFormat;
            Channels = channels;
            SampleRate = sampleRate;
            BitsPerSample = bitsPerSample;
            ByteRate = byteRate;
            BlockAlign = blockAlign;

            ThrowIfInvalid();
        }

        private void ThrowIfInvalid()
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            var expectedByteRate = SampleRate * Channels * BitsPerSample / 8;
            if (ByteRate != expectedByteRate)
            {
                throw new ArgumentException($"The given byte rate {ByteRate} is not expected, expected was {expectedByteRate}", nameof(ByteRate));
            }

            var expectedBlockAlign = (ushort)(Channels * BitsPerSample / 8);
            if (BlockAlign != expectedBlockAlign)
            {
                throw new ArgumentException($"The given block align {BlockAlign} is not expected, expected was {expectedBlockAlign}", nameof(BlockAlign));
            }

            if (ChunkId != ChunkIdentifier)
            {
                throw new ArgumentException(ExceptionMessages.FmtSubChunkIdMissmatch, nameof(ChunkId));
            }

            if (ChunkSize != FmtChunkSize)
            {
                throw new ArgumentException(ExceptionMessages.FmtSubChunckSizeMissmatch, nameof(ChunkSize));
            }

            if (Type != AudioFormatType.Pcm && Type != AudioFormatType.IeeFloat)
            {
                throw new ArgumentException(ExceptionMessages.FmtSubChunckFormatNotSupported, nameof(Type));
            }
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
        }
    }
}
