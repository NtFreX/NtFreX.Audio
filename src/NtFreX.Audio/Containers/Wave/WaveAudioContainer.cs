using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Infrastructure.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Wave
{
    public sealed class WaveAudioContainer : RiffContainer, IWaveAudioContainer
    {
        public static readonly int DefaultHeaderSize = 36;

        public IReadOnlyList<UnknownSubChunk> UnknownSubChunks { get; private set; }
        public FmtSubChunk FmtSubChunk { get; private set; }
        public DataSubChunk DataSubChunk { get; private set; }

        public WaveAudioContainer(IRiffSubChunk riffSubChunk, FmtSubChunk fmtSubChunk, DataSubChunk dataSubChunk, IReadOnlyList<UnknownSubChunk> riffSubChunks)
            : base(riffSubChunk, new ISubChunk[] { fmtSubChunk, dataSubChunk }.Concat(riffSubChunks).ToList())
        {
            FmtSubChunk = fmtSubChunk;
            DataSubChunk = dataSubChunk;
            UnknownSubChunks = riffSubChunks;
        }

        public IAudioFormat GetFormat()
            => FmtSubChunk;
        public TimeSpan GetLength()
            => TimeSpan.FromSeconds(DataSubChunk.ChunkSize / (FmtSubChunk.ByteRate * 1.0f));
        public long GetByteLength()
            => DataSubChunk.ChunkSize;
        public void Dispose() { }
        public ValueTask DisposeAsync()
            => DataSubChunk.DisposeAsync();

        public ISeekableAsyncEnumerator<IReadOnlyList<byte>?> GetAsyncAudioEnumerator(CancellationToken cancellationToken = default)
            => DataSubChunk.GetAsyncEnumerator(cancellationToken);
    }
}