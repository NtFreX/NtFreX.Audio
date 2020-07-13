using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Containers
{
    /// <summary>
    /// http://soundfile.sapp.org/doc/WaveFormat/
    /// </summary>
    //TODO: support seekable and non seekable cases
    public sealed class WaveStreamAudioContainer : WaveAudioContainer<StreamDataSubChunk>, IWaveStreamAudioContainer
    {
        private bool disposed = false;

        IFmtSubChunk IWaveStreamAudioContainer.FmtSubChunk => FmtSubChunk;

        public WaveStreamAudioContainer([NotNull] RiffChunkDescriptor riffChunkDescriptor, [NotNull] FmtSubChunk fmtSubChunk, [NotNull] StreamDataSubChunk dataSubChunk, [NotNull] IReadOnlyList<UnknownSubChunk> riffSubChuncks)
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

        [return: NotNull] internal WaveStreamAudioContainer WithRiffChunkDescriptor([NotNull] Func<RiffChunkDescriptor, RiffChunkDescriptor> riffChunkDescriptor) => new WaveStreamAudioContainer(riffChunkDescriptor(RiffChunkDescriptor), FmtSubChunk, DataSubChunk, UnknownSubChuncks);
        [return: NotNull] internal WaveStreamAudioContainer WithFmtSubChunk([NotNull] Func<FmtSubChunk, FmtSubChunk> fmtSubChunk) => new WaveStreamAudioContainer(RiffChunkDescriptor, fmtSubChunk(FmtSubChunk), DataSubChunk, UnknownSubChuncks);
        [return: NotNull] internal WaveStreamAudioContainer WithDataSubChunk([NotNull] Func<StreamDataSubChunk, StreamDataSubChunk> dataSubChunk) => new WaveStreamAudioContainer(RiffChunkDescriptor, FmtSubChunk, dataSubChunk(DataSubChunk), UnknownSubChuncks);
        [return: NotNull] internal WaveStreamAudioContainer WithRiffSubChunks([NotNull] UnknownSubChunk[] riffSubChunks) => new WaveStreamAudioContainer(RiffChunkDescriptor, FmtSubChunk, DataSubChunk, riffSubChunks);
    }
}
