using System;

namespace NtFreX.Audio.Wasapi.Interop
{
    /// <summary>
    /// Audio Client Buffer Flags
    /// </summary>
    [Flags]
    internal enum AudioClientBufferFlags
    {
        None,
        DataDiscontinuity = 0x1,
        Silent = 0x2,
        TimestampError = 0x4
    }
}
