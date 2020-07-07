using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using NtFreX.Audio.Helpers;

namespace NtFreX.Audio.Containers
{
    /// <summary>
    /// http://soundfile.sapp.org/doc/WaveFormat/
    /// </summary>
    //TODO: support seekable and non seekable cases
    public class WaveAudioContainer : AudioContainer
    {
        private bool _disposed = false;

        public IReadOnlyList<UnknownSubChunk> UnknownSubChuncks { [return:NotNull] get; private set; }
        public RiffChunkDescriptor RiffChunkDescriptor { [return: NotNull] get; private set; }
        public FmtSubChunk FmtSubChunk { [return: NotNull] get; private set; }
        public DataSubChunk DataSubChunk { [return: NotNull] get; private set; }

        [return: NotNull] public WaveAudioContainer WithRiffChunkDescriptor([NotNull] Func<RiffChunkDescriptor, RiffChunkDescriptor> riffChunkDescriptor) => new WaveAudioContainer(riffChunkDescriptor(RiffChunkDescriptor), FmtSubChunk, DataSubChunk, UnknownSubChuncks);
        [return: NotNull] public WaveAudioContainer WithFmtSubChunk([NotNull] Func<FmtSubChunk, FmtSubChunk> fmtSubChunk) => new WaveAudioContainer(RiffChunkDescriptor, fmtSubChunk(FmtSubChunk), DataSubChunk, UnknownSubChuncks);
        [return: NotNull] public WaveAudioContainer WithDataSubChunk([NotNull] Func<DataSubChunk, DataSubChunk> dataSubChunk) => new WaveAudioContainer(RiffChunkDescriptor, FmtSubChunk, dataSubChunk(DataSubChunk), UnknownSubChuncks);
        [return: NotNull] public WaveAudioContainer WithRiffSubChunks([NotNull] UnknownSubChunk[] riffSubChunks) => new WaveAudioContainer(RiffChunkDescriptor, FmtSubChunk, DataSubChunk, riffSubChunks);

        public WaveAudioContainer([NotNull] RiffChunkDescriptor riffChunkDescriptor, [NotNull] FmtSubChunk fmtSubChunk, [NotNull] DataSubChunk dataSubChunk, [NotNull] IReadOnlyList<UnknownSubChunk> riffSubChuncks)
        {
            RiffChunkDescriptor = riffChunkDescriptor;
            FmtSubChunk = fmtSubChunk;
            DataSubChunk = dataSubChunk;
            UnknownSubChuncks = riffSubChuncks;
        }

        [return: NotNull] public override TimeSpan GetLength()
            => TimeSpan.FromSeconds(DataSubChunk.Subchunk2Size / (FmtSubChunk.ByteRate * 1.0f));
        private int GetSampleSize()
            => FmtSubChunk.BitsPerSample / 8;
        private int GetChannelSize()
            => (int) (DataSubChunk.Subchunk2Size / GetSampleSize() / FmtSubChunk.NumChannels);
        public bool IsDataLittleEndian() 
            => RiffChunkDescriptor.ChunkId == RiffChunkDescriptor.RIFF;

        [return: NotNull]
        public async IAsyncEnumerable<byte[]> GetAudioSamplesAsync([MaybeNull] [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var samplesSize = GetSampleSize();
            await foreach(var buffer in GetAudioSamplesAsBufferAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
            {
                for(var i = 0; i < buffer.Length; i += samplesSize)
                {
                    yield return buffer.AsMemory(i, samplesSize).ToArray();
                }
            }
        }
        [return: NotNull]
        private async IAsyncEnumerable<byte[]> GetAudioSamplesAsBufferAsync([MaybeNull] [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var readContext = await DataSubChunk.Data.AquireAsync(cancellationToken).ConfigureAwait(false);

            var bufferSize = StreamFactory.GetBufferSize();
            var max = DataSubChunk.Subchunk2Size;
            var current = 0;
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException("A cancelation has been requested");
                }

                var realBufferSize = (int)(current + bufferSize > max ? max - current : bufferSize);
                var buffer = new byte[realBufferSize];

                try
                {
                    var readLength = await readContext.Data.ReadAsync(buffer, 0, realBufferSize, cancellationToken).ConfigureAwait(false);
                    if (readLength == 0)
                    {
                        break;
                    }
                }
                catch (Exception exce)
                {
                    throw new Exception("Loading audio samples failed", exce);
                }

                yield return buffer;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                DataSubChunk.Dispose();
            }

            base.Dispose(disposing);
            _disposed = true;
        }
    }
}
