using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Interop
{
    [ComImport]
    [Guid(ClsId.IAudioClock)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioClock
    {
        [PreserveSig]
        HResult GetFrequency([Out] out ulong pu64Frequency);

        [PreserveSig]
        HResult GetPosition([Out] out ulong pu64Position, [Out] out ulong pu64QPCPosition);
    }
}
