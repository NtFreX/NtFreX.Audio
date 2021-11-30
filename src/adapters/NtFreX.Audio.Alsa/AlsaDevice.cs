using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Alsa.Wrapper;
using NtFreX.Audio.Infrastructure;
using System;

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
        
        public AudioFormat GetDefaultFormat()
        {
            throw new NotImplementedException();
        }

        public bool TryInitialize(IAudioFormat format, out IAudioClient? client, out IAudioFormat supportedFormat)
        {
            _ = format ?? throw new ArgumentNullException(nameof(format));

            if (!ManagedAlsaAudioClient.TryInitialize(GetManagedDevice(), format, out supportedFormat))
            {
                client = null;
                return false;
            }

            client = new AlsaAudioClient(this);
            supportedFormat = format;
            return true;
        }

        public void Dispose()
            => managedAlsaDevice.Dispose();
    }
}
