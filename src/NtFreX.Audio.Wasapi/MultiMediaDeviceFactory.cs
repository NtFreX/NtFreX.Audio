using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Wasapi.Wrapper;

namespace NtFreX.Audio.Wasapi
{
    public class MultiMediaDeviceFactory : IAudioDeviceFactory
    {
        public IAudioDevice GetDefaultRenderDevice()
            => MultiMediaDevice.FromManaged(ManagedMultiMediaDeviceEnumerator.Instance.GetDefaultRenderDevice());
    }
}
