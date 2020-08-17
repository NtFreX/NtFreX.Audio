using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NtFreX.Audio.Containers
{
    public abstract class WaveAudioContainer<TData> : RiffContainer, IWaveAudioContainer
        where TData : IDataSubChunk
    {
        public static readonly int DefaultHeaderSize = 36;

        public IReadOnlyList<UnknownSubChunk> UnknownSubChunks { [return: NotNull] get; private set; }
        public FmtSubChunk FmtSubChunk { [return: NotNull] get; private set; }
        public TData DataSubChunk { [return: NotNull] get; private set; }

        public IAudioFormat Format => FmtSubChunk;
       
        protected WaveAudioContainer([NotNull] IRiffSubChunk riffSubChunk, [NotNull] FmtSubChunk fmtSubChunk, [NotNull] TData dataSubChunk, [NotNull] IReadOnlyList<UnknownSubChunk> riffSubChunks)
            : base(riffSubChunk, new ISubChunk[] { fmtSubChunk, dataSubChunk }.Concat(riffSubChunks).ToList())
        {
            FmtSubChunk = fmtSubChunk;
            DataSubChunk = dataSubChunk;
            UnknownSubChunks = riffSubChunks;
        }

        [return: NotNull]
        public TimeSpan GetLength()
            => TimeSpan.FromSeconds(DataSubChunk.ChunkSize / (FmtSubChunk.ByteRate * 1.0f));

        public void SeekTo(TimeSpan time)
        {
            var length = GetLength();
            var position = time;
            DataSubChunk.SeekTo((long) (position / length * DataSubChunk.ChunkSize));
        }

        [return: NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IAsyncEnumerable<Sample> GetAudioSamplesAsync([MaybeNull] CancellationToken cancellationToken = default)
            => DataSubChunk.GetAudioSamplesAsync(cancellationToken);
    }
}
