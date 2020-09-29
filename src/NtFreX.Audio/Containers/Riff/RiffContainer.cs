using NtFreX.Audio.Infrastructure.Container;
using System.Collections.Generic;
using System.Linq;

namespace NtFreX.Audio.Containers
{
    public abstract class RiffContainer: IRiffContainer
    {
        private IReadOnlyList<ISubChunk> subChuncks;

        public IRiffSubChunk RiffSubChunk { get; private set; }
        public IReadOnlyList<ISubChunk> SubChunks 
        { 
            get => subChuncks; 
            private set
            {
                subChuncks = value;
                RiffSubChunk = RiffSubChunk.WithChunkSize((uint)(subChuncks.Sum(x => x.ChunkSize + 8 /* ChunkId + ChunkSize */) + 4 /* RiffSubChunk.Format */));
            }
        }

        public bool IsDataLittleEndian()
            => RiffSubChunk.IsDataLittleEndian();

        protected RiffContainer(IRiffSubChunk riffSubChunk, IReadOnlyList<ISubChunk> subChunks)
        {
            RiffSubChunk = riffSubChunk;
            SubChunks = subChunks;

            this.subChuncks = subChunks;
        }
    }
}
