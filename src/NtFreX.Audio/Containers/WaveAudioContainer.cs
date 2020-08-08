using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
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

        public bool IsDataLittleEndian()
            => RiffSubChunk.ChunkId == Containers.RiffSubChunk.ChunkIdentifierRIFF;

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
