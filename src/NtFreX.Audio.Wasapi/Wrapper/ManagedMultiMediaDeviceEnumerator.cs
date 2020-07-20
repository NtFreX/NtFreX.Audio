using NtFreX.Audio.Wasapi.Interop;
using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Wrapper
{
    internal class ManagedMultiMediaDeviceEnumerator : IDisposable
    {
        private readonly IMMDeviceEnumerator deviceEnumerator;

        public static ManagedMultiMediaDeviceEnumerator Instance { get; } = new ManagedMultiMediaDeviceEnumerator();

        private ManagedMultiMediaDeviceEnumerator()
        {
            deviceEnumerator = ComObject.Initialize<IMMDeviceEnumerator>(ClsId.MMDeviceEnumerator);
        }

        public ManagedMultiMediaDevice GetDefaultRenderDevice()
        {
            /*
             * In Windows Vista, the MMDevice API supports device roles but the system-supplied user interface programs do not. 
             * The user interface in Windows Vista enables the user to select a default audio device for rendering and a default audio device for capture. 
             * When the user changes the default rendering or capture device, the system assigns all three device roles (eConsole, eMultimedia, and eCommunications) 
             * to that device. Thus, GetDefaultAudioEndpoint always selects the default rendering or capture device, 
             * regardless of which role is indicated by the role parameter. In a future version of Windows, the user interface might enable the user 
             * to assign individual roles to different devices. In that case, the selection of a rendering or capture device by GetDefaultAudioEndpoint 
             * might depend on the role parameter. Thus, the behavior of an audio application developed to run in Windows Vista might change 
             * when run in a future version of Windows.
            */
            if (deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia, out IMMDevice device) != HResult.S_OK || device == null)
            {
                throw new Exception("Could not get the default renderer device");
            }

            return new ManagedMultiMediaDevice(device);
        }

        public void Dispose()
        {
            Marshal.ReleaseComObject(deviceEnumerator);
        }
    }
}
