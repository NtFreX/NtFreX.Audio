using NtFreX.Audio.Alsa.Interop;
using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Alsa.Wrapper
{
    public static class HResultExtensions
    {
        public static void ThrowIfNotSucceded(this HResult result, string message = "An unknown interop exception occured")
        {
            if(result == 0)
            {
                return;
            }

            var resultCode = (int)result;
            var errorPtr = Interop.Alsa.snd_strerror(resultCode);
            var errorMsg = errorPtr != null && errorPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(errorPtr) : "Unknown interop error code";

            throw new Exception(message, new COMException(errorMsg, resultCode));
        }
    }
}
