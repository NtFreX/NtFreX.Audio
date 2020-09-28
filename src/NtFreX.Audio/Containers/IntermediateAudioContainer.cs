using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers
{
    public abstract class IntermediateAudioContainer : IIntermediateAudioContainer
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
        public abstract void Dispose();

        public IAudioFormat GetFormat()
            => format;
        public bool IsDataLittleEndian()
            => isLittleEndian;

        public ISeekableAsyncEnumerable<IReadOnlyList<byte>> GetAsyncAudioEnumerable(CancellationToken cancellationToken = default)
            => this.SelectAsync(x => (IReadOnlyList<byte>)x.AsByteArray().ToList(), cancellationToken);

        public abstract ISeekableAsyncEnumerator<Sample> GetAsyncEnumerator(CancellationToken cancellationToken = default);
    }
}
