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
    public sealed class StreamDataSubChunk : DataSubChunk<StreamDataSubChunk>, IDisposable
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

        public StreamDataSubChunk(long startIndex, string chunkId, uint chunkSize, Stream data)
            : base(chunkId, chunkSize)
        {
            StartIndex = startIndex;
            Data = new ReadLock<Stream>(data, data => OnStreamAquire(data, startIndex));
        }

        [return: NotNull]
        public override async IAsyncEnumerable<byte[]> GetAudioSamplesAsBufferAsync([MaybeNull][EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var readContext = await Data.AquireAsync(cancellationToken).ConfigureAwait(false);
            if (readContext.Data == null)
            {
                throw new Exception(ExceptionMessages.DataStreamNull);
            }

            var bufferSize = StreamFactory.GetBufferSize();
            var endOfChunk = ChunkSize + StartIndex + ChunkHeaderSize;
            var buffer = new byte[bufferSize];
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(ExceptionMessages.CancelationRequested);
                }

                if (readContext.Data.Position + bufferSize > endOfChunk)
                {
                    // other chunks could follow after this so stop when end of chunk is reached
                    var endBufferSize = (int)(endOfChunk - readContext.Data.Position);
                    await readContext.Data.ReadAsync(buffer.AsMemory(0, endBufferSize), cancellationToken).ConfigureAwait(false);
                    yield return buffer.AsMemory(0, endBufferSize).ToArray();
                    break;
                }
                else
                {
                    await readContext.Data.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
                    yield return buffer;
                }
            }
        }

        public void Dispose()
        {
            Data.Dispose();
        }

        public override void SeekTo(long position)
        {
            var index = StartIndex + ChunkHeaderSize + position;
            var stream = Data.UnsafeAccess();
            if(stream == null)
            {
                throw new Exception("Stream is null");
            }

            if (!stream.CanSeek)
            {
                throw new Exception("The stream is non seekable an therefore can only be read once");
            }

            stream.Seek(index, SeekOrigin.Begin);
        }

        [return: NotNull] public override StreamDataSubChunk WithChunkId([NotNull] string chunkId) => throw new Exception("Stream containers are read only");
        [return: NotNull] public override StreamDataSubChunk WithChunkSize(uint chunkSize) => throw new Exception("Stream containers are read only");

        private static void OnStreamAquire(Stream? stream, long startIndex)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            var dataStartIndex = startIndex + ChunkHeaderSize;
            if (stream.Position == dataStartIndex)
            {
                return;
            }

            if (!stream.CanSeek)
            {
                throw new Exception("The stream is non seekable an therefore can only be read once");
            }

            stream.Seek(dataStartIndex, SeekOrigin.Begin);
        }
    }
}
