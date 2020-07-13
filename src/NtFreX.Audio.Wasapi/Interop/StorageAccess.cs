using System;

namespace NtFreX.Audio.Wasapi.Interop
{
    [Flags]
    internal enum StorageAccess: uint
    {
        Read = 0,
        Write = 1,
        ReadWrite = 2
    }
}
