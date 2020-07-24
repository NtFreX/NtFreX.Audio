using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Interop
{
    [ComImport]
    [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDevice
    {
        [PreserveSig]
        HResult Activate([In] Guid iid, [In] ClsCtx clsctx, [In] IntPtr activationParams, [Out, MarshalAs(UnmanagedType.IUnknown)] out object interfacePointer);

        [PreserveSig]
        HResult OpenPropertyStore([In] object access, [Out] out object propertystore);

        [PreserveSig]
        HResult GetId([Out, MarshalAs(UnmanagedType.LPWStr)] out string deviceId);

        [PreserveSig]
        HResult GetState([Out] out DeviceState state);
    }
}
