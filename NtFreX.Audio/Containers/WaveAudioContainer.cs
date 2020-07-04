using NtFreX.Audio.Samplers;
using NtFreX.Audio.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;

namespace NtFreX.Audio.Containers
{
    /// <summary>
    /// http://soundfile.sapp.org/doc/WaveFormat/
    /// </summary>
    public class WaveAudioContainer : AudioContainer
    {
        public RiffSubChunk[] RiffSubChuncks { get; }
        public RiffChunkDescriptor RiffChunkDescriptor { get; }
        public FmtSubChunk FmtSubChunk { get; }
        public DataSubChunk DataSubChunk { get; }

        public WaveAudioContainer WithRiffChunkDescriptor(Func<RiffChunkDescriptor, RiffChunkDescriptor> riffChunkDescriptor) => new WaveAudioContainer(riffChunkDescriptor(RiffChunkDescriptor), FmtSubChunk, DataSubChunk, RiffSubChuncks);
        public WaveAudioContainer WithFmtSubChunk(Func<FmtSubChunk, FmtSubChunk> fmtSubChunk) => new WaveAudioContainer(RiffChunkDescriptor, fmtSubChunk(FmtSubChunk), DataSubChunk, RiffSubChuncks);
        public WaveAudioContainer WithDataSubChunk(Func<DataSubChunk, DataSubChunk> dataSubChunk) => new WaveAudioContainer(RiffChunkDescriptor, FmtSubChunk, dataSubChunk(DataSubChunk), RiffSubChuncks);
        public WaveAudioContainer WithRiffSubChunks(RiffSubChunk[] riffSubChunks) => new WaveAudioContainer(RiffChunkDescriptor, FmtSubChunk, DataSubChunk, riffSubChunks);

        public WaveAudioContainer(RiffChunkDescriptor riffChunkDescriptor, FmtSubChunk fmtSubChunk, DataSubChunk dataSubChunk, RiffSubChunk[] riffSubChuncks)
        {
            RiffChunkDescriptor = riffChunkDescriptor;
            FmtSubChunk = fmtSubChunk;
            DataSubChunk = dataSubChunk;
            RiffSubChuncks = riffSubChuncks;
        }

        public override TimeSpan GetLength()
            => TimeSpan.FromSeconds(DataSubChunk.Subchunk2Size / (FmtSubChunk.ByteRate * 1.0f));

        private int GetSampleSize()
            => FmtSubChunk.BitsPerSample / 8;

        private int GetChannelSize()
            => (int) (DataSubChunk.Subchunk2Size / GetSampleSize() / FmtSubChunk.NumChannels);

        // TODO: make work
        public async Task<WaveAudioContainer> SampleToMonoAsync(CancellationToken cancellationToken = default)
        {
            if (FmtSubChunk.NumChannels == 1)
            {
                throw new InvalidOperationException("The channels are already mono");
            }

            var samples = GetAudioSamplesAsync(cancellationToken: cancellationToken);
            var interleavedData = await MonoAudioSampler.InterleaveChannelDataAsync(samples, FmtSubChunk.NumChannels).ConfigureAwait(false);

            byte[] monoData = interleavedData.Select(x => BitConverter.GetBytes((short)x)).SelectMany(x => x).ToArray();
            return WithFmtSubChunk(x => x.WithNumChannels(1))
                  .WithDataSubChunk(x => x
                    .WithData(new MemoryStream(monoData))
                    .WithSubchunk2Size((uint) monoData.Length));
        }

        public async IAsyncEnumerable<byte[]> GetAudioSamplesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
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

        public bool IsDataLittleEndian() => RiffChunkDescriptor.GetChunkId() == RiffChunkDescriptor.RIFF;

        private async IAsyncEnumerable<byte[]> GetAudioSamplesAsBufferAsync(int bufferSizeInSamples = 100, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var readContext = await DataSubChunk.Data.AquireAsync(cancellationToken).ConfigureAwait(false);

            var samplesSize = GetSampleSize() * bufferSizeInSamples;
            var max = DataSubChunk.Subchunk2Size;
            var current = 0;
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException("A cancelation has been requested");
                }

                var bufferSize = (int)(current + samplesSize > max ? max - current : samplesSize);
                var buffer = new byte[bufferSize];

                try
                {
                    var readLength = await readContext.Data.ReadAsync(buffer, 0, bufferSize, cancellationToken).ConfigureAwait(false);
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

    }
}
