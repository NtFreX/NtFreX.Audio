using NtFreX.Audio.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers
{
    public class DataSubChunk : IDisposable
    {
        public const string DATA = "data";

        public int StartIndex { get; }

        /// <summary>
        /// Contains the letters "data" (0x64617461 big-endian form).
        /// </summary>
        public int Subchunk2Id { get; }

        /// <summary>
        /// == NumSamples * NumChannels * BitsPerSample/8
        /// </summary>
        public uint Subchunk2Size { get; }

        /// <summary>
        /// The actual sound data.
        /// </summary>
        public ReadLock<Stream> Data { get; }

        public DataSubChunk WithSubchunk2Id(int subchunk2Id) => new DataSubChunk(StartIndex, subchunk2Id, Subchunk2Size, Data.AquireAndDisposeOrThrow());
        public DataSubChunk WithSubchunk2Size(uint subchunk2Size) => new DataSubChunk(StartIndex, Subchunk2Id, subchunk2Size, Data.AquireAndDisposeOrThrow());
        public DataSubChunk WithData(Stream data) => new DataSubChunk(StartIndex, Subchunk2Id, Subchunk2Size, data);

        public DataSubChunk(int startIndex, int subchunk2Id, uint subchunk2Size, Stream data)
        {
            StartIndex = startIndex;
            Subchunk2Id = subchunk2Id;
            Subchunk2Size = subchunk2Size;
            Data = new ReadLock<Stream>(data, data => data.Seek(this.StartIndex + 8, SeekOrigin.Begin));

            ThrowIfInvalid();
        }

        private void ThrowIfInvalid()
        {
            if (GetSubchunk2Id() != DATA)
            {
                throw new ArgumentException("The value has to contain the letters 'data' (0x64617461 big-endian form)", nameof(Subchunk2Id));
            }
        }

        public string GetSubchunk2Id() => Encoding.ASCII.GetString(BitConverter.GetBytes(Subchunk2Id).Reverse().ToArray());

        public void Dispose()
        {
            Data.Dispose();
        }
    }
}
