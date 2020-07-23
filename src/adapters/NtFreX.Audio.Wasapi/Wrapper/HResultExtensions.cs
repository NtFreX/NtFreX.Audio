using NtFreX.Audio.Wasapi.Interop;
using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Wrapper
{
    internal static class HResultExtensions
    {
        public static void ThrowIfNotSucceded(this HResult result, string message = "An unknown com exception occured")
        {
            if (result != HResult.S_OK)
            {
                throw new Exception(message, new COMException(result.ToString(), (int)result));
            }
        }
    }
}
