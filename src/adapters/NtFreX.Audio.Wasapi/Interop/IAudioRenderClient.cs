using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Interop
{
    [ComImport]
    [Guid(ClsId.IAudioRendererClient)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioRenderClient
    {
        [PreserveSig]
        HResult GetBuffer(uint numFramesRequested, out IntPtr dataBufferPointer);

        [PreserveSig]
        HResult ReleaseBuffer(uint numFramesWritten, AudioClientBufferFlags bufferFlags);
    }
}
