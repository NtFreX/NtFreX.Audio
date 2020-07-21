using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Resources;
using System;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Containers
{
    public sealed class FmtSubChunk : IFmtSubChunk
    {
        public const string ChunkIdentifier = "fmt ";

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
        public AudioFormatType AudioFormat { get; }

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
        public uint ByteRate => SampleRate * Channels * BitsPerSample / 8;

        /// <summary>
        /// == NumChannels * BitsPerSample/8
        /// </summary>
        public ushort BlockAlign => (ushort) (Channels * BitsPerSample / 8);

        /// <summary>
        /// 8 bits = 8, 16 bits = 16, etc.
        /// </summary>
        public ushort BitsPerSample { get; }

        [return: NotNull] public FmtSubChunk WithChunkId([NotNull] string chunkId) => new FmtSubChunk(chunkId, ChunkSize, AudioFormat, Channels, SampleRate/*, ByteRate, BlockAlign*/, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithChunkSize(uint chunkSize) => new FmtSubChunk(ChunkId, chunkSize, AudioFormat, Channels, SampleRate/*, ByteRate, BlockAlign*/, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithAudioFormat(AudioFormatType audioFormat) => new FmtSubChunk(ChunkId, ChunkSize, audioFormat, Channels, SampleRate/*, ByteRate, BlockAlign*/, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithChannels(ushort channels) => new FmtSubChunk(ChunkId, ChunkSize, AudioFormat, channels, SampleRate/*, ByteRate, BlockAlign*/, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithSampleRate(uint sampleRate) => new FmtSubChunk(ChunkId, ChunkSize, AudioFormat, Channels, sampleRate/*, ByteRate, BlockAlign*/, BitsPerSample);
        //public FmtSubChunk WithByteRate(uint byteRate) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, AudioFormat, NumChannels, SampleRate/*, byteRate, BlockAlign*/, BitsPerSample);
        // public FmtSubChunk WithBlockAlign(ushort blockAlign) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, AudioFormat, NumChannels, SampleRate, ByteRate, blockAlign, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithBitsPerSample(ushort bitsPerSample) => new FmtSubChunk(ChunkId, ChunkSize, AudioFormat, Channels, SampleRate/*, ByteRate, BlockAlign*/, bitsPerSample);

        public FmtSubChunk([NotNull] string chunkId, uint chunkSize, AudioFormatType audioFormat, ushort channels, uint sampleRate /*, uint byteRate, ushort blockAlign */, ushort bitsPerSample)
        {
            ChunkId = chunkId;
            ChunkSize = chunkSize;
            AudioFormat = audioFormat;
            Channels = channels;
            SampleRate = sampleRate;
            BitsPerSample = bitsPerSample;

            ThrowIfInvalid(/*byteRate, blockAlign*/);
        }

        private void ThrowIfInvalid(/*uint byteRate, ushort blockAlign*/)
        {
            //TODO: validate byteRate and blockAlign
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            /*if (byteRate != ByteRate)
            {
                throw new ArgumentException("The given value is wrong", nameof(ByteRate));
            }

            if (blockAlign != BlockAlign)
            {
                throw new ArgumentException("The given value is wrong", nameof(ByteRate));
            }*/

            if (ChunkId != ChunkIdentifier)
            {
                throw new ArgumentException(ExceptionMessages.FmtSubChunkIdMissmatch, nameof(ChunkId));
            }

            // TODO: chunk size should be 24
            if (ChunkSize != 16)
            {
                throw new ArgumentException(ExceptionMessages.FmtSubChunckSizeMissmatch, nameof(ChunkSize));
            }

            // TODO: validate/support ieefloat
            if (AudioFormat != AudioFormatType.PCM)
            {
                throw new ArgumentException(ExceptionMessages.FmtSubChunckFormatNotSupported, nameof(AudioFormat));
            }
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
        }
    }
}
