using NtFreX.Audio.Wasapi.Interop;
using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Wrapper
{
    internal class ManagedWaveFormat : IDisposable
    {
        public WaveFormatDefinition Unmanaged { get; }
        public IntPtr Ptr { get; }

        internal ManagedWaveFormat(WaveFormatDefinition format)
        {
            Unmanaged = format;
            Ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(Unmanaged));
            Marshal.StructureToPtr(Unmanaged, Ptr, false);
        }

        internal ManagedWaveFormat(IntPtr ptr)
        {
            Ptr = ptr;
            Unmanaged = Marshal.PtrToStructure<WaveFormatDefinition>(ptr);
        }

        public void Dispose()
        {
            Marshal.FreeCoTaskMem(Ptr);
        }
    }
}
