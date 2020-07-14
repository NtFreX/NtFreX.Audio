using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers
{
    public abstract class WaveAudioContainer<TData> : IWaveAudioContainer
        where TData : DataSubChunk
    {
        public IReadOnlyList<UnknownSubChunk> UnknownSubChuncks { [return: NotNull] get; private set; }
        public RiffChunkDescriptor RiffChunkDescriptor { [return: NotNull] get; private set; }
        public FmtSubChunk FmtSubChunk { [return: NotNull] get; protected set; }
        public TData DataSubChunk { [return: NotNull] get; private set; }

        IFmtSubChunk IWaveAudioContainer.FmtSubChunk => FmtSubChunk;

        protected WaveAudioContainer([NotNull] RiffChunkDescriptor riffChunkDescriptor, [NotNull] FmtSubChunk fmtSubChunk, [NotNull] TData dataSubChunk, [NotNull] IReadOnlyList<UnknownSubChunk> riffSubChuncks)
        {
            RiffChunkDescriptor = riffChunkDescriptor;
            FmtSubChunk = fmtSubChunk;
            DataSubChunk = dataSubChunk;
            UnknownSubChuncks = riffSubChuncks;
        }

        [return: NotNull]
        public TimeSpan GetLength()
            => TimeSpan.FromSeconds(DataSubChunk.Subchunk2Size / (FmtSubChunk.ByteRate * 1.0f));
        public bool IsDataLittleEndian()
            => RiffChunkDescriptor.ChunkId == RiffChunkDescriptor.ChunkIdentifier;

        [return: NotNull]
        public async IAsyncEnumerable<byte[]> GetAudioSamplesAsync([MaybeNull][EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var samplesSize = GetSampleSize();
            await foreach (var buffer in DataSubChunk.GetAudioSamplesAsBufferAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                for (var i = 0; i < buffer.Length; i += samplesSize)
                {
                    yield return buffer.AsMemory(i, samplesSize).ToArray();
                }
            }
        }

        private int GetSampleSize()
            => FmtSubChunk.BitsPerSample / 8;
    }
}
