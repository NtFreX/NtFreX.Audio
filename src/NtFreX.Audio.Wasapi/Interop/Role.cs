using System;

namespace NtFreX.Audio.Wasapi.Interop
{
    [Flags]
    internal enum Role: uint
    {
        Console = 0,
        Multimedia = 1,
        Communications = 2,
    }
}
