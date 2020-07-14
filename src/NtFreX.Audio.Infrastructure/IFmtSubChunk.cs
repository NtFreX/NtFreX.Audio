namespace NtFreX.Audio.Infrastructure
{
    public interface IFmtSubChunk
    {
        uint ByteRate { get; }
        uint SampleRate { get; }
        ushort NumChannels { get; }
        ushort BitsPerSample { get; }
    }
}
