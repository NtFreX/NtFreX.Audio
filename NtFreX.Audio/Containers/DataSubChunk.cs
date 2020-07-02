using System;

namespace NtFreX.Audio.Containers
{
    public class DataSubChunk
    {
        /// <summary>
        /// Contains the letters "data" (0x64617461 big-endian form).
        /// </summary>
        public int Subchunk2Id { get; }

        /// <summary>
        /// == NumSamples * NumChannels * BitsPerSample/8
        /// </summary>
        public int Subchunk2Size { get; }

        /// <summary>
        /// The actual sound data.
        /// </summary>
        public byte[] Data { get; }

        public DataSubChunk WithSubchunk2Id(int subchunk2Id) => new DataSubChunk(subchunk2Id, Subchunk2Size, Data);
        public DataSubChunk WithSubchunk2Size(int subchunk2Size) => new DataSubChunk(Subchunk2Id, subchunk2Size, Data);
        public DataSubChunk WithData(byte[] data) => new DataSubChunk(Subchunk2Id, Subchunk2Size, data);

        public DataSubChunk(int subchunk2Id, int subchunk2Size, byte[] data)
        {
            Subchunk2Id = subchunk2Id;
            Subchunk2Size = subchunk2Size;
            Data = data;

            ThrowIfInvalid();
        }

        private void ThrowIfInvalid()
        {
            if (BitConverter.ToString(BitConverter.GetBytes(Subchunk2Id)) != "4C-49-53-54")
            {
                throw new ArgumentException("The value has to contain the letters 'data' (0x64617461 big-endian form)", nameof(Subchunk2Id));
            }
        }
    }
}
