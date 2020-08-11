using NtFreX.Audio.Infrastructure.Container;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Containers
{
    /// <summary>
    /// http://soundfile.sapp.org/doc/WaveFormat/
    /// </summary>
    public sealed class WaveStreamAudioContainer : WaveAudioContainer<StreamDataSubChunk>, IStreamAudioContainer
    {
        private bool disposed = false;

        public WaveStreamAudioContainer([NotNull] IRiffSubChunk riffChunkDescriptor, [NotNull] FmtSubChunk fmtSubChunk, [NotNull] StreamDataSubChunk dataSubChunk, [NotNull] IReadOnlyList<UnknownSubChunk> riffSubChuncks)
            : base(riffChunkDescriptor, fmtSubChunk, dataSubChunk, riffSubChuncks) { }
        
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            DataSubChunk.Dispose();
            disposed = true;
        }
    }
}
