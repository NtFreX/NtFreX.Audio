using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers
{
#pragma warning disable CA1063 // Implement IDisposable Correctly => no implementation is provided
    public abstract class IntermediateAudioContainer : IIntermediateAudioContainer
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        private readonly bool isLittleEndian;
        private readonly IAudioFormat format;

        protected IntermediateAudioContainer(IAudioFormat format, bool isLittleEndian)
        {
            this.format = format;
            this.isLittleEndian = isLittleEndian;
        }

        public long GetByteLength()
            => GetDataLength() * GetFormat().BytesPerSample;
        public abstract long GetDataLength();
        public abstract TimeSpan GetLength();
        
        public abstract ValueTask DisposeAsync();
#pragma warning disable CA1063 // Implement IDisposable Correctly => no implentation is provided
        public abstract void Dispose();
#pragma warning restore CA1063 // Implement IDisposable Correctly

        public IAudioFormat GetFormat()
            => format;
        public bool IsDataLittleEndian()
            => isLittleEndian;

        public ISeekableAsyncEnumerable<Memory<byte>> GetAsyncAudioEnumerable(CancellationToken cancellationToken = default)
            => this.SelectAsync(x => x.AsByteArray(), cancellationToken);

        public abstract ISeekableAsyncEnumerator<Sample> GetAsyncEnumerator(CancellationToken cancellationToken = default);
    }
}
