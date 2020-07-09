using NtFreX.Audio.Helpers;
using NtFreX.Audio.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NtFreX.Audio.Containers
{
    public sealed class StreamDataSubChunk : DataSubChunk, IDisposable
    {
        /// <summary>
        /// Start index of this package. 
        /// It is used to seek the given stream to the start of the audio.
        /// </summary>
        public long StartIndex { get; }

        /// <summary>
        /// The actual sound data wrapped in a single access lock.
        /// </summary>
        public ReadLock<Stream> Data { get; }

        public StreamDataSubChunk(long startIndex, [NotNull] string subchunk2Id, uint subchunk2Size, [NotNull] Stream data)
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting. => set by overloaded constructor
            : this(startIndex, subchunk2Id, subchunk2Size, new ReadLock<Stream>(data, data => data?.Seek(startIndex + ChunkHeaderSize, SeekOrigin.Begin))) { }
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.

        internal StreamDataSubChunk(long startIndex, [NotNull] string subchunk2Id, uint subchunk2Size, [NotNull] ReadLock<Stream> data)
            : base(subchunk2Id, subchunk2Size)
        {
            StartIndex = startIndex;
            Data = data;
        }

        [return: NotNull]
        public override async IAsyncEnumerable<byte[]> GetAudioSamplesAsBufferAsync([MaybeNull][EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var readContext = await Data.AquireAsync(cancellationToken).ConfigureAwait(false);
            if(readContext.Data == null)
            {
                throw new Exception(ExceptionMessages.DataStreamNull);
            }

            var bufferSize = StreamFactory.GetBufferSize();
            var max = Subchunk2Size;
            var current = 0;
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(ExceptionMessages.CancelationRequested);
                }

                var realBufferSize = (int)(current + bufferSize > max ? max - current : bufferSize);
                var buffer = new byte[realBufferSize];

                try
                {
                    var readLength = await readContext.Data.ReadAsync(buffer, 0, realBufferSize, cancellationToken).ConfigureAwait(false);
                    if (readLength == 0)
                    {
                        break;
                    }
                }
                catch (Exception exce)
                {
                    throw new Exception(ExceptionMessages.AudioSampleLoadingFailed, exce);
                }

                yield return buffer;
            }
        }

        public void Dispose()
        {
            Data.Dispose();
        }

        [return: NotNull] internal StreamDataSubChunk WithSubchunk2Id([NotNull] string subchunk2Id) => new StreamDataSubChunk(StartIndex, subchunk2Id, Subchunk2Size, Data);
        [return: NotNull] internal StreamDataSubChunk WithSubchunk2Size(uint subchunk2Size) => new StreamDataSubChunk(StartIndex, Subchunk2Id, subchunk2Size, Data);
        [return: NotNull] internal StreamDataSubChunk WithData([NotNull] Stream data) => new StreamDataSubChunk(StartIndex, Subchunk2Id, Subchunk2Size, data);
    }
}
