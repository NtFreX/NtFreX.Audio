using NtFreX.Audio.Wasapi.Interop;
using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Wrapper
{
    internal class ManagedMultiMediaDevice : IDisposable
    {
        private readonly IMMDevice device;

        internal ManagedMultiMediaDevice(IMMDevice device)
        {
            this.device = device;
        }

        public string GetId()
        {
            if (device.GetId(out var id) != HResult.S_OK || string.IsNullOrEmpty(id))
            {
                throw new Exception("Could not get the device id");
            }

            return id;
        }

        public ManagedAudioClient Activate()
        {
            if (device.Activate(new Guid(ClsId.IAudioClient), ClsCtx.LocalServer, IntPtr.Zero, out object audioClientObj) != HResult.S_OK || !(audioClientObj is IAudioClient audioClient) || audioClient == null)
            {
                throw new Exception("Could not activate device");
            }

            return new ManagedAudioClient(audioClient);
        }

        public void Dispose()
        {
            Marshal.ReleaseComObject(device);
        }
    }
}
