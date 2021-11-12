namespace NtFreX.Audio.Infrastructure
{
    public interface IAudioFormat
    {
        uint SampleRate { get; }
        ushort BitsPerSample { get; }
        ushort Channels { get; }
        AudioFormatType Type { get; }

        uint ByteRate => SampleRate * Channels * BytesPerSample;
        ushort BytesPerSample => (ushort) (BitsPerSample / 8);
        ushort BlockAlign => (ushort)(Channels * BytesPerSample);
    }
}
