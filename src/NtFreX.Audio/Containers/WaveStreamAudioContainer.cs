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

        [return: NotNull] internal WaveStreamAudioContainer WithRiffSubChunk([NotNull] Func<IRiffSubChunk, IRiffSubChunk> riffSubChunk) => new WaveStreamAudioContainer(riffSubChunk(RiffSubChunk), FmtSubChunk, DataSubChunk, UnknownSubChunks);
        [return: NotNull] internal WaveStreamAudioContainer WithFmtSubChunk([NotNull] Func<FmtSubChunk, FmtSubChunk> fmtSubChunk) => new WaveStreamAudioContainer(RiffSubChunk, fmtSubChunk(FmtSubChunk), DataSubChunk, UnknownSubChunks);
        [return: NotNull] internal WaveStreamAudioContainer WithDataSubChunk([NotNull] Func<StreamDataSubChunk, StreamDataSubChunk> dataSubChunk) => new WaveStreamAudioContainer(RiffSubChunk, FmtSubChunk, dataSubChunk(DataSubChunk), UnknownSubChunks);
        // TODO: update start index in data chunk if size changed
        [return: NotNull] internal WaveStreamAudioContainer WithRiffSubChunks([NotNull] UnknownSubChunk[] riffSubChunks) => new WaveStreamAudioContainer(RiffSubChunk, FmtSubChunk, DataSubChunk, riffSubChunks);
    }
}
