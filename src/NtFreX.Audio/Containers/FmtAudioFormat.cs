using NtFreX.Audio.Infrastructure;
using System;

namespace NtFreX.Audio.Containers
{
    public class FmtAudioFormat : IAudioFormat
    {
        private readonly Func<uint> getSampleRate;
        private readonly Func<ushort> getBitsPerSample;
        private readonly Func<ushort> getChannels;
        private readonly Func<AudioFormatType> getType;

        public uint SampleRate => getSampleRate();

        public ushort BitsPerSample => getBitsPerSample();

        public ushort Channels => getChannels();

        public AudioFormatType Type => getType();

        public FmtAudioFormat(Func<uint> getSampleRate, Func<ushort> getBitsPerSample, Func<ushort> getChannels, Func<AudioFormatType> getType)
        {
            this.getSampleRate = getSampleRate;
            this.getBitsPerSample = getBitsPerSample;
            this.getChannels = getChannels;
            this.getType = getType;
        }
    }
}
