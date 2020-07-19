namespace NtFreX.Audio.Infrastructure
{
    public interface IFmtSubChunk : ISubChunk
    {
        uint ByteRate { get; }
        uint SampleRate { get; }
        ushort NumChannels { get; }
        ushort BitsPerSample { get; }
    }
}
