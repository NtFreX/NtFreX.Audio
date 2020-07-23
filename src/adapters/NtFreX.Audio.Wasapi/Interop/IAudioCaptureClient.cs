using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Interop
{
    [ComImport]
    [Guid(ClsId.IAudioCaptureClient)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioCaptureClient
    {
        [PreserveSig]
        HResult GetBuffer(
            [Out] out IntPtr data,
            [Out] out uint numFramesToRead,
            [Out] out AudioClientBufferFlags flags,
            [Out] out ulong devicePosition,
            [Out] out ulong qpcPosition);

        [PreserveSig]
        HResult ReleaseBuffer(uint numFramesRead);

        [PreserveSig]
        HResult GetNextPacketSize(out uint numFramesInNextPacket);
    }
}
