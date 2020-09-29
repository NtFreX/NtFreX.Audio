namespace NtFreX.Audio.Infrastructure
{
    public sealed class AudioFormat : IAudioFormat
    {
        public uint SampleRate { get; }
        public ushort BitsPerSample { get; }
        public ushort Channels { get; }
        public AudioFormatType Type { get; }

        public AudioFormat(uint sampleRate, ushort bitPerSample, ushort channels, AudioFormatType type)
        {
            SampleRate = sampleRate;
            BitsPerSample = bitPerSample;
            Channels = channels;
            Type = type;
        }
    }
}
