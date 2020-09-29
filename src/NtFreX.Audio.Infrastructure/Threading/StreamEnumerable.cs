﻿using NtFreX.Audio.Infrastructure.Helpers;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    public sealed class StreamEnumerable : ISeekableAsyncEnumerable<Memory<byte>>, IAsyncDisposable
    {
        private readonly ReadLock<Stream> stream;
        private readonly long startIndex;
        private readonly long endIndex;

        public StreamEnumerable(Stream stream, long startIndex, long endIndex)
        {
            this.stream = new ReadLock<Stream>(stream, data => SeekTo(data, startIndex));
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }

        public static long GetDataLength(long startIndex, long endIndex)
        {
            var inBytes = endIndex - startIndex;
            var bufferSize = StreamFactory.GetBufferSize();
            var rest = inBytes % bufferSize;
            return (inBytes / bufferSize) + (rest > 0 ? 1 : 0);
        }

        public long GetDataLength()
            => GetDataLength(startIndex, endIndex);

        public ISeekableAsyncEnumerator<Memory<byte>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            if (!stream.TryAquire(out var readLockContext) || readLockContext == null)
            {
                throw new NotSupportedException("The stream is allready in use");
            }

            return new StreamEnumerator(readLockContext, startIndex, endIndex, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            var context = await stream.AquireAsync().ConfigureAwait(false);

            _ = context.Data ?? throw new ArgumentNullException();

            await context.Data.DisposeAsync().ConfigureAwait(false);

            context.Dispose();
            stream.Dispose();
        }

        private static void SeekTo(Stream? stream, long startIndex, long position = 0)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            var target = startIndex + position;
            if (stream.Position == target)
            {
                return;
            }

            if (!stream.CanSeek)
            {
                throw new Exception("The stream is non seekable an therefore can only be read once");
            }

            stream.Seek(target, SeekOrigin.Begin);
        }
    }
}