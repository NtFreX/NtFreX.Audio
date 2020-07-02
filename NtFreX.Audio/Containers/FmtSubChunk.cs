using System;

namespace NtFreX.Audio.Containers
{
    public class FmtSubChunk
    {
        /// <summary>
        /// Contains the letters "fmt" (0x666d7420 big-endian form).
        /// </summary>
        public int Subchunk1Id { get; }

        /// <summary>
        /// 16 for PCM.  This is the size of the rest of the Subchunk which follows this number.
        /// </summary>
        public int Subchunk1Size { get; }

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
        public int SampleRate { get; }

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

        public FmtSubChunk WithSubchunk1Id(int subchunk1Id) => new FmtSubChunk(subchunk1Id, Subchunk1Size, AudioFormat, NumChannels, SampleRate, ByteRate, BlockAlign, BitsPerSample);
        public FmtSubChunk WithSubchunk1Size(int subchunk1Size) => new FmtSubChunk(Subchunk1Id, subchunk1Size, AudioFormat, NumChannels, SampleRate, ByteRate, BlockAlign, BitsPerSample);
        public FmtSubChunk WithAudioFormat(ushort audioFormat) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, audioFormat, NumChannels, SampleRate, ByteRate, BlockAlign, BitsPerSample);
        public FmtSubChunk WithNumChannels(ushort numChannels) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, AudioFormat, numChannels, SampleRate, ByteRate, BlockAlign, BitsPerSample);
        public FmtSubChunk WithSampleRate(int sampleRate) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, AudioFormat, NumChannels, sampleRate, ByteRate, BlockAlign, BitsPerSample);
        public FmtSubChunk WithByteRate(uint byteRate) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, AudioFormat, NumChannels, SampleRate, byteRate, BlockAlign, BitsPerSample);
        public FmtSubChunk WithBlockAlign(ushort blockAlign) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, AudioFormat, NumChannels, SampleRate, ByteRate, blockAlign, BitsPerSample);
        public FmtSubChunk WithBitsPerSample(ushort bitsPerSample) => new FmtSubChunk(Subchunk1Id, Subchunk1Size, AudioFormat, NumChannels, SampleRate, ByteRate, BlockAlign, bitsPerSample);

        public FmtSubChunk(int subchunk1Id, int subchunk1Size, ushort audioFormat, ushort numChannels, int sampleRate, uint byteRate, ushort blockAlign, ushort bitsPerSample)
        {
            Subchunk1Id = subchunk1Id;
            Subchunk1Size = subchunk1Size;
            AudioFormat = audioFormat;
            NumChannels = numChannels;
            SampleRate = sampleRate;
            ByteRate = byteRate;
            BlockAlign = blockAlign;
            BitsPerSample = bitsPerSample;

            ThrowIfInvalid();
        }

        private void ThrowIfInvalid()
        {
            if (BitConverter.ToString(BitConverter.GetBytes(Subchunk1Id)) != "66-6D-74-20")
            {
                throw new ArgumentException("The value has to contain the letters 'fmt' (0x666d7420 big-endian form)", nameof(Subchunk1Id));
            }

            if (Subchunk1Size != 16)
            {
                throw new ArgumentException("Only pcm containers with a fmt chunk size of 16 are supported", nameof(Subchunk1Size));
            }

            if (AudioFormat != 1)
            {
                throw new ArgumentException("Only pcm containers no compression are supported", nameof(AudioFormat));
            }
        }
    }
}
