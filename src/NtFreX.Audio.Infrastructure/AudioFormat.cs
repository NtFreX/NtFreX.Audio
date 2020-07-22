namespace NtFreX.Audio.Infrastructure
{
    //TODO: change interfaces and work with one format? (make it possible to interact without wave audioContainer)
    public sealed class AudioFormat
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
