﻿using NtFreX.Audio.Helpers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NtFreX.Audio.Containers
{
    public sealed class StreamDataSubChunk : DataSubChunk<StreamDataSubChunk>, IDisposable
    {
        /// <summary>
        /// Start index of this package. 
        /// It is used to seek the given stream to the start of the audio.
        /// </summary>
        //TODO: update startIndex when data before this chunk changes
        public long StartIndex { get; }

        /// <summary>
        /// The actual sound data wrapped in a single access lock.
        /// </summary>
        public ReadLock<Stream> Data { get; }

        public StreamDataSubChunk(long startIndex, [NotNull] string chunkId, uint chunkSize, [NotNull] Stream data)
#pragma warning disable CS8777 // Parameter must have a non-null value when exiting. => set by overloaded constructor
            : this(startIndex, chunkId, chunkSize, new ReadLock<Stream>(data, data => OnStreamAquire(data, startIndex))) { }
#pragma warning restore CS8777 // Parameter must have a non-null value when exiting.

        internal StreamDataSubChunk(long startIndex, [NotNull] string chunkId, uint chunkSize, [NotNull] ReadLock<Stream> data)
            : base(chunkId, chunkSize)
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
            var max = ChunkSize;
            var current = 0;
            var endOfChunk = ChunkSize + StartIndex + ChunkHeaderSize;
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(ExceptionMessages.CancelationRequested);
                }

                var realBufferSize = (int)(current + bufferSize > max ? max - current : bufferSize);
                if (readContext.Data.Position + realBufferSize > endOfChunk)
                {
                    // other chunks could follow after this so stop when end of chunk is reached
                    realBufferSize = (int) (endOfChunk - readContext.Data.Position);
                }

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

        [return: NotNull] public override StreamDataSubChunk WithChunkId([NotNull] string chunkId) => new StreamDataSubChunk(StartIndex, chunkId, ChunkSize, Data);
        [return: NotNull] public override StreamDataSubChunk WithChunkSize(uint chunkSize) => new StreamDataSubChunk(StartIndex, ChunkId, chunkSize, Data);
        [return: NotNull] public StreamDataSubChunk WithData([NotNull] Stream data) => new StreamDataSubChunk(StartIndex, ChunkId, ChunkSize, data);

        private static void OnStreamAquire(Stream? stream, long startIndex) 
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            var dataStartIndex = startIndex + ChunkHeaderSize;
            if (stream.Position == dataStartIndex)
            {
                return;
            }

            if(!stream.CanSeek)
            {
                throw new Exception("The stream is non seekable an therefore can only be read once");
            }

            stream.Seek(dataStartIndex, SeekOrigin.Begin);
        }
    }
}
