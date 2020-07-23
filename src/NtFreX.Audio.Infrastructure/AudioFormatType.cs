namespace NtFreX.Audio.Infrastructure
{
#pragma warning disable CA1028 // Enum Storage should be Int32 => compatibility with win32
    public enum AudioFormatType : ushort
#pragma warning restore CA1028 // Enum Storage should be Int32
    {
        Pcm = 1,
        IeeFloat = 0x0003
    }
}
