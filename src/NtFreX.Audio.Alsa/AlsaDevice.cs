using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Alsa.Wrapper;

namespace NtFreX.Audio.Alsa
{
    public sealed class AlsaDevice : IAudioDevice
    {
        private readonly ManagedAlsaDevice managedAlsaDevice;

        internal AlsaDevice(ManagedAlsaDevice managedAlsaDevice)
        {
            this.managedAlsaDevice = managedAlsaDevice;
        }

        internal ManagedAlsaDevice GetManagedDevice() => managedAlsaDevice;

        public string GetId()
            => managedAlsaDevice.Id;

        public void Dispose()
            => managedAlsaDevice.Dispose();
    }
}
