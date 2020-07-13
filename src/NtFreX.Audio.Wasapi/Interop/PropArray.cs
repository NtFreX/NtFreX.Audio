using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Interop
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal struct PropArray
    {
        internal UInt32 cElems;
        internal IntPtr pElems;
    }
}
