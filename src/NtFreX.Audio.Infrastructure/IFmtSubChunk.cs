namespace NtFreX.Audio.Infrastructure
{
    public interface IFmtSubChunk : ISubChunk
    {
        uint ByteRate { get; }
        uint SampleRate { get; }
        ushort Channels { get; }
        ushort BitsPerSample { get; }
        AudioFormatType AudioFormat { get; }
    }
}
