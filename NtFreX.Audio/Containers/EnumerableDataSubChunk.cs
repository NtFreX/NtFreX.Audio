using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Containers
{
    public sealed class EnumerableDataSubChunk : DataSubChunk
    {
        /// <summary>
        /// The actual sound data.
        /// </summary>
        public IAsyncEnumerable<byte[]> Data { get; }

        [return: NotNull] internal EnumerableDataSubChunk WithSubchunk2Id([NotNull] string subchunk2Id) => new EnumerableDataSubChunk(subchunk2Id, Subchunk2Size, Data);
        [return: NotNull] internal EnumerableDataSubChunk WithSubchunk2Size(uint subchunk2Size) => new EnumerableDataSubChunk(Subchunk2Id, subchunk2Size, Data);
        [return: NotNull] internal EnumerableDataSubChunk WithData([NotNull] IAsyncEnumerable<byte[]> data) => new EnumerableDataSubChunk(Subchunk2Id, Subchunk2Size, data);

        public EnumerableDataSubChunk([NotNull] string subchunk2Id, uint subchunk2Size, [NotNull] IAsyncEnumerable<byte[]> data)
            : base(subchunk2Id, subchunk2Size)
        {
            Data = data;
        }

        [return: NotNull]
        public override IAsyncEnumerable<byte[]> GetAudioSamplesAsBufferAsync([MaybeNull] CancellationToken cancellationToken = default)
            => Data;
    }
}
