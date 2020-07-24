using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Alsa.Wrapper;

namespace NtFreX.Audio.Alsa
{
    public class AlsaDeviceFactory : IAudioDeviceFactory
    {
        public IAudioDevice GetDefaultCaptureDevice()
            => new AlsaDevice(ManagedAlsaDeviceEnumerator.GetDefaultCaptureDevice());

        public IAudioDevice GetDefaultRenderDevice()
            => new AlsaDevice(ManagedAlsaDeviceEnumerator.GetDefaultRenderDevice());
    }
}
