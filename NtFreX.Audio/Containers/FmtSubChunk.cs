using NtFreX.Audio.Resources;
using System;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Containers
{
    public sealed class FmtSubChunk
    {
        public const string ChunkIdentifier = "fmt ";

        /// <summary>
        /// Contains the letters "fmt" (0x666d7420 big-endian form).
        /// </summary>
        public string Subchunk1Id { get; }

        /// <summary>
        /// 16 for PCM.  This is the size of the rest of the Subchunk which follows this number.
        /// </summary>
        public uint Subchunk1Size { get; }

        /// <summary>
        /// PCM = 1 (i.e. Linear quantization) Values other than 1 indicate some form of compression.
        /// </summary>
        public ushort AudioFormat { get; }

        /// <summary>
        /// Mono = 1, Stereo = 2, etc.
        /// </summary>
        public ushort NumChannels { get; }

        /// <summary>
        /// 8000, 44100, etc.
        /// </summary>
        public uint SampleRate { get; }

        /// <summary>
        /// == SampleRate * NumChannels * BitsPerSample/8
        /// </summary>
        public uint ByteRate => SampleRate * NumChannels * BitsPerSample / 8;

        /// <summary>
        /// == NumChannels * BitsPerSample/8
        /// </summary>
        public ushort BlockAlign => (ushort) (NumChannels * BitsPerSample / 8);

        /// <summary>
        /// 8 bits = 8, 16 bits = 16, etc.
        /// </summary>
        public ushort BitsPerSample { get; }

        [return: NotNull] public FmtSubChunk WithSubchunk1Id([NotNull] string subchunk1Id) => new FmtSubChunk(subchunk1Id, Subchunk1Size, AudioFormat, NumChannels, SampleRate/*, ByteRate, BlockAlign*/, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithSubchunk1Size(uint subchunk1Size) => new FmtSubChunk(Subchunk1Id, subchunk1Size, AudioFormat, NumChannels, SampleRate/*, ByteRate, BlockAlign*/, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithAudioFormat(ushort audioFormat) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, audioFormat, NumChannels, SampleRate/*, ByteRate, BlockAlign*/, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithNumChannels(ushort numChannels) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, AudioFormat, numChannels, SampleRate/*, ByteRate, BlockAlign*/, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithSampleRate(uint sampleRate) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, AudioFormat, NumChannels, sampleRate/*, ByteRate, BlockAlign*/, BitsPerSample);
        //public FmtSubChunk WithByteRate(uint byteRate) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, AudioFormat, NumChannels, SampleRate/*, byteRate, BlockAlign*/, BitsPerSample);
        // public FmtSubChunk WithBlockAlign(ushort blockAlign) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, AudioFormat, NumChannels, SampleRate, ByteRate, blockAlign, BitsPerSample);
        [return: NotNull] public FmtSubChunk WithBitsPerSample(ushort bitsPerSample) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, AudioFormat, NumChannels, SampleRate/*, ByteRate, BlockAlign*/, bitsPerSample);

        public FmtSubChunk([NotNull] string subchunk1Id, uint subchunk1Size, ushort audioFormat, ushort numChannels, uint sampleRate /*, uint byteRate, ushort blockAlign */, ushort bitsPerSample)
        {
            Subchunk1Id = subchunk1Id;
            Subchunk1Size = subchunk1Size;
            AudioFormat = audioFormat;
            NumChannels = numChannels;
            SampleRate = sampleRate;
            BitsPerSample = bitsPerSample;

            ThrowIfInvalid(/*byteRate, blockAlign*/);
        }

        private void ThrowIfInvalid(/*uint byteRate, ushort blockAlign*/)
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            /*if (byteRate != ByteRate)
            {
                throw new ArgumentException("The given value is wrong", nameof(ByteRate));
            }

            if (blockAlign != BlockAlign)
            {
                throw new ArgumentException("The given value is wrong", nameof(ByteRate));
            }*/

            if (Subchunk1Id != ChunkIdentifier)
            {
                throw new ArgumentException(ExceptionMessages.FmtSubChunkIdMissmatch, nameof(Subchunk1Id));
            }

            if (Subchunk1Size != 16)
            {
                throw new ArgumentException(ExceptionMessages.FmtSubChunckSizeMissmatch, nameof(Subchunk1Size));
            }

            if (AudioFormat != 1)
            {
                throw new ArgumentException(ExceptionMessages.FmtSubChunckFormatNotSupported, nameof(AudioFormat));
            }
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
        }
    }
}
