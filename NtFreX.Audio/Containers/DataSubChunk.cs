using NtFreX.Audio.Helpers;
using NtFreX.Audio.Resources;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NtFreX.Audio.Containers
{
    public sealed class DataSubChunk : IDisposable
    {
        public const string DATA = "data";

        public long StartIndex { get; }

        /// <summary>
        /// Contains the letters "data" (0x64617461 big-endian form).
        /// </summary>
        public string Subchunk2Id { [return:NotNull] get; }

        /// <summary>
        /// == NumSamples * NumChannels * BitsPerSample/8
        /// </summary>
        public uint Subchunk2Size { get; }

        /// <summary>
        /// The actual sound data.
        /// </summary>
        public ReadLock<Stream> Data { [return: NotNull] get; }

        [return: NotNull] internal DataSubChunk WithSubchunk2Id([NotNull] string subchunk2Id) => new DataSubChunk(StartIndex, subchunk2Id, Subchunk2Size, Data);
        [return: NotNull] internal DataSubChunk WithSubchunk2Size(uint subchunk2Size) => new DataSubChunk(StartIndex, Subchunk2Id, subchunk2Size, Data);
        [return: NotNull] public DataSubChunk WithData([NotNull] Stream data) => new DataSubChunk(StartIndex, Subchunk2Id, Subchunk2Size, data);

        public DataSubChunk(long startIndex, [NotNull] string subchunk2Id, uint subchunk2Size, [NotNull] Stream data)
            : this(startIndex, subchunk2Id, subchunk2Size, new ReadLock<Stream>(data, data => data.Seek(startIndex + 8, SeekOrigin.Begin))) { }

        internal DataSubChunk(long startIndex, [NotNull] string subchunk2Id, uint subchunk2Size, [NotNull] ReadLock<Stream> data)
        {
            StartIndex = startIndex;
            Subchunk2Id = subchunk2Id;
            Subchunk2Size = subchunk2Size;
            Data = data;

            ThrowIfInvalid();
        }

        private void ThrowIfInvalid()
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            if (Subchunk2Id != DATA)
            {
                throw new ArgumentException(ExceptionMessages.DataSubChunkIdMissmatch, nameof(Subchunk2Id));
            }
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
        }

        public void Dispose()
        {
            Data.Dispose();
        }
    }
}
