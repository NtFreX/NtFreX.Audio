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
    public sealed class StreamBufferSubChunk : IDisposable
    {
        /// <summary>
        /// Start index of this package. 
        /// It is used to seek the given stream to the start of the audio.
        /// </summary>
        public long StartIndex { get; }
        public string ChunkId { get; }
        public uint ChunkSize { get; }

        /// <summary>
        /// The actual sound data wrapped in a single access lock.
        /// </summary>
        public ReadLock<Stream> Data { get; }

        public StreamBufferSubChunk(long startIndex, string chunkId, uint chunkSize, Stream data)
        {
            StartIndex = startIndex;
            ChunkId = chunkId;
            ChunkSize = chunkSize;
            Data = new ReadLock<Stream>(data, data => SeekTo(data, startIndex));
        }

        public void Dispose()
        {
            Data.Dispose();
        }

        public void SeekTo(long position)
            => SeekTo(Data.UnsafeAccess(), StartIndex, position);

        [return: NotNull]
        public async IAsyncEnumerable<byte[]> GetAudioSamplesAsBufferAsync([MaybeNull][EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var readContext = await Data.AquireAsync(cancellationToken).ConfigureAwait(false);
            if (readContext.Data == null)
            {
                throw new Exception(ExceptionMessages.DataStreamNull);
            }

            var bufferSize = StreamFactory.GetBufferSize();
            var endOfChunk = ChunkSize + StartIndex + DataSubChunk<StreamDataSubChunk>.ChunkHeaderSize;
            var buffer = new byte[bufferSize];
            while (true)
            {
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

        private static void SeekTo(Stream? stream, long startIndex, long position = 0)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            var index = startIndex + DataSubChunk<StreamDataSubChunk>.ChunkHeaderSize + position;
            if (stream.Position == index)
            {
                return;
            }

            if (!stream.CanSeek)
            {
                throw new Exception("The stream is non seekable an therefore can only be read once");
            }

            stream.Seek(index, SeekOrigin.Begin);
        }
    }
}
