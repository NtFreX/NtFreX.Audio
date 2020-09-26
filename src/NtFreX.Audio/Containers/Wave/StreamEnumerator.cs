using NtFreX.Audio.Helpers;
using NtFreX.Audio.Infrastructure.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Wave
{
    public sealed class StreamEnumerator : ISeekableAsyncEnumerator<IReadOnlyList<byte>>
    {
        private readonly ReadLockContext<Stream> readLockContext;
        private readonly long startIndex;
        private readonly long endIndex;
        private readonly CancellationToken cancellationToken;

        public IReadOnlyList<byte> Current { get; private set; }

        public StreamEnumerator(ReadLockContext<Stream> readLockContext, long startIndex, long endIndex, CancellationToken cancellationToken)
        {
            Current = default!;

            this.readLockContext = readLockContext;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.cancellationToken = cancellationToken;
        }

        public ValueTask DisposeAsync()
        {
            readLockContext.Dispose();
            return new ValueTask(Task.CompletedTask);
        }

        public async ValueTask<bool> MoveNextAsync()
        {
            _ = readLockContext.Data ?? throw new ArgumentNullException();

            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            var bufferSize = StreamFactory.GetBufferSize();
            var buffer = new byte[readLockContext.Data.Position + bufferSize > endIndex 
                ? endIndex - readLockContext.Data.Position
                : bufferSize];

            var size = await readLockContext.Data.ReadAsync(buffer).ConfigureAwait(false);
            if(size == 0)
            {
                Current = default!;
                return false;
            }

            Current = buffer.Take(size).ToList();
            return true;
        }

        public long GetDataLength()
            => endIndex - startIndex;
        public bool CanSeek()
            => readLockContext.Data?.CanSeek ?? false;
        public void SeekTo(long position)
            => readLockContext.Data?.Seek(startIndex + position, SeekOrigin.Begin);
        public long GetPosition()
            => readLockContext.Data?.Position ?? 0 - startIndex;
    }
}
