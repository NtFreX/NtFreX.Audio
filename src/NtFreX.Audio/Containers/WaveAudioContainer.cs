using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers
{
    public abstract class WaveAudioContainer<TData> : IWaveAudioContainer
        where TData : DataSubChunk
    {
        public static readonly int DefaultHeaderSize = 36;

        public IReadOnlyList<UnknownSubChunk> UnknownSubChuncks { [return: NotNull] get; private set; }
        public RiffChunkDescriptor RiffChunkDescriptor { [return: NotNull] get; private set; }
        public FmtSubChunk FmtSubChunk { [return: NotNull] get; private set; }
        public TData DataSubChunk { [return: NotNull] get; private set; }

        public IAudioFormat Format => FmtSubChunk;
       
        protected WaveAudioContainer([NotNull] RiffChunkDescriptor riffChunkDescriptor, [NotNull] FmtSubChunk fmtSubChunk, [NotNull] TData dataSubChunk, [NotNull] IReadOnlyList<UnknownSubChunk> riffSubChuncks)
        {
            RiffChunkDescriptor = riffChunkDescriptor;
            FmtSubChunk = fmtSubChunk;
            DataSubChunk = dataSubChunk;
            UnknownSubChuncks = riffSubChuncks;
        }

        [return: NotNull]
        public TimeSpan GetLength()
            => TimeSpan.FromSeconds(DataSubChunk.ChunkSize / (FmtSubChunk.ByteRate * 1.0f));

        public bool IsDataLittleEndian()
            => RiffChunkDescriptor.ChunkId == RiffChunkDescriptor.ChunkIdentifierRIFF;

        [return: NotNull]
        public async IAsyncEnumerable<Sample> GetAudioSamplesAsync([MaybeNull][EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var samplesSize = FmtSubChunk.BitsPerSample / 8;
            var isLittleEndian = IsDataLittleEndian();
            var definition = new SampleDefinition(FmtSubChunk.Type, FmtSubChunk.BitsPerSample, isLittleEndian);
            var tempBuffer = new List<byte>();
            await foreach (var buffer in DataSubChunk.GetAudioSamplesAsBufferAsync(cancellationToken: cancellationToken).WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                tempBuffer.AddRange(buffer);
                while(tempBuffer.Count >= samplesSize)
                {
                    yield return new Sample(tempBuffer.Take(samplesSize).ToArray(), definition);
                    tempBuffer.RemoveRange(0, samplesSize);
                }
            }

            Debug.Assert(tempBuffer.Count == 0, $"Temp buffer should be completly returned and emptied but is {tempBuffer.Count} long");
        }
    }
}
