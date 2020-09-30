using NtFreX.Audio.Infrastructure.Helpers;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    public sealed class StreamEnumerator : ISeekableAsyncEnumerator<Memory<byte>>
    {
        private readonly byte[] buffer = new byte[StreamFactory.GetBufferSize()];
        private readonly ReadLockContext<Stream> readLockContext;
        private readonly long startIndex;
        private readonly long endIndex;
        private readonly CancellationToken cancellationToken;

        public Memory<byte> Current { get; private set; }

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
            var realBufferSize = readLockContext.Data.Position + bufferSize > endIndex
                ? endIndex - readLockContext.Data.Position
                : bufferSize;
            var memory = buffer.AsMemory(0, (int) realBufferSize);

            var task = readLockContext.Data.ReadAsync(memory);
            int size;
            if(task.IsCompleted)
            {
                size = task.GetAwaiter().GetResult();
            }
            else
            {
                size = await task.ConfigureAwait(false);
            }

            if(size == 0)
            {
                Current = default!;
                return false;
            }

            Current = memory;
            return true;
        }

        // TODO: delete this or in stream enumerable
        public long GetDataLength()
            => StreamEnumerable.GetDataLength(startIndex, endIndex);

        public bool CanSeek()
            => readLockContext.Data?.CanSeek ?? false;
        
        public void SeekTo(long position)
        {
            var positionInBytes = StreamFactory.GetBufferSize() * position;
            readLockContext.Data?.Seek(startIndex + positionInBytes, SeekOrigin.Begin);
        }

        public long GetPosition()
        {
            var bufferSize = StreamFactory.GetBufferSize();
            var positionInBytes = readLockContext.Data?.Position ?? 0 - startIndex;
            return (long) (1.0f * positionInBytes / bufferSize);
        }
    }
}
