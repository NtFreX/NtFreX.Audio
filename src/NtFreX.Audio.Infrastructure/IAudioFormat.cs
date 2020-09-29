namespace NtFreX.Audio.Infrastructure
{
    public interface IAudioFormat
    {
        uint SampleRate { get; }
        ushort BitsPerSample { get; }
        ushort Channels { get; }
        AudioFormatType Type { get; }

        ushort BytesPerSample => (ushort) (BitsPerSample / 8);
    }
}
