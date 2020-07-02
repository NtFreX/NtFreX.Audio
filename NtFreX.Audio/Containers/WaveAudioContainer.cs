using NtFreX.Audio.Extensions;
using NtFreX.Audio.Samplers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NtFreX.Audio.Containers
{
    /// <summary>
    /// http://soundfile.sapp.org/doc/WaveFormat/
    /// </summary>
    public class WaveAudioContainer : AudioContainer
    {
        public RiffChunkDescriptor RiffChunkDescriptor { get; }
        public FmtSubChunk FmtSubChunk { get; }
        public DataSubChunk DataSubChunk { get; }

        public WaveAudioContainer WithRiffChunkDescriptor(Func<RiffChunkDescriptor, RiffChunkDescriptor> riffChunkDescriptor) => new WaveAudioContainer(riffChunkDescriptor(RiffChunkDescriptor), FmtSubChunk, DataSubChunk);
        public WaveAudioContainer WithFmtSubChunk(Func<FmtSubChunk, FmtSubChunk> fmtSubChunk) => new WaveAudioContainer(RiffChunkDescriptor, fmtSubChunk(FmtSubChunk), DataSubChunk);
        public WaveAudioContainer WithDataSubChunk(Func<DataSubChunk, DataSubChunk> dataSubChunk) => new WaveAudioContainer(RiffChunkDescriptor, FmtSubChunk, dataSubChunk(DataSubChunk));

        public WaveAudioContainer(RiffChunkDescriptor riffChunkDescriptor, FmtSubChunk fmtSubChunk, DataSubChunk dataSubChunk)
        {
            RiffChunkDescriptor = riffChunkDescriptor;
            FmtSubChunk = fmtSubChunk;
            DataSubChunk = dataSubChunk;
        }

        public override TimeSpan GetLength()
            => TimeSpan.FromSeconds(DataSubChunk.Data.Length / (FmtSubChunk.ByteRate * 1.0f));

        public byte[] GetAudioData()
            => DataSubChunk.Data;

        private int GetSampleSize()
            => FmtSubChunk.BitsPerSample / 8;

        private int GetChannelSize()
            => DataSubChunk.Data.Length / GetSampleSize() / FmtSubChunk.NumChannels;

        // TODO: make work
        public WaveAudioContainer SampleToMono()
        {
            if (FmtSubChunk.NumChannels == 1)
            {
                throw new InvalidOperationException("The channels are already mono");
            }

            var channels = GetAudioSamplesPerChannel().ToInt(IsDataLittleEndian());
            var interleavedData = MonoAudioSampler.InterleaveChannelData(channels);

            byte[] monoData = interleavedData.Select(x => BitConverter.GetBytes((short)x)).SelectMany(x => x).ToArray();
            return WithFmtSubChunk(x => x
                        .WithNumChannels(1)
                        .WithBlockAlign((ushort)(FmtSubChunk.BitsPerSample / 8))
                        .WithByteRate((uint)(FmtSubChunk.SampleRate * FmtSubChunk.BitsPerSample / 8)))
                    .WithDataSubChunk(x => x
                        .WithData(monoData)
                        .WithSubchunk2Size(monoData.Length));
        }

        public IEnumerable<byte[]>[] GetAudioSamplesPerChannel()
        {
            var channelSize = GetChannelSize();
            var audioSamples = GetAudioSamples();
            var channels = new IEnumerable<byte[]>[FmtSubChunk.NumChannels];
            for (var channelIndex = 0; channelIndex < FmtSubChunk.NumChannels; channelIndex++)
            {
                channels[channelIndex] = audioSamples.Skip(channelIndex * channelSize).Take(channelSize);
            }
            return channels;
        }

        public IEnumerable<byte[]> GetAudioSamples()
        {
            var samplesSize = GetSampleSize();
            for (int current = 0; current < DataSubChunk.Data.Length; current += samplesSize)
            {
                yield return DataSubChunk.Data.Skip(current).Take(samplesSize).ToArray();
            }
        }

        public bool IsDataLittleEndian() => RiffChunkDescriptor.GetChunkId() == RiffChunkDescriptor.RIFF;
    }
}
