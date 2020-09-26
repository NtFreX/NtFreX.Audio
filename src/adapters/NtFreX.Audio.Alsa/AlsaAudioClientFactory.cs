using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Alsa.Wrapper;
using NtFreX.Audio.Infrastructure;
using System;

namespace NtFreX.Audio.Alsa
{
    public class AlsaAudioClientFactory : IAudioClientFactory
    {
        public AudioFormat GetDefaultFormat(IAudioDevice device)
        {
            throw new System.NotImplementedException();
        }

        public bool TryInitialize(IAudioFormat format, IAudioDevice device, out IAudioClient? client, out IAudioFormat supportedFormat)
        {
            _ = format ?? throw new ArgumentNullException(nameof(format));

            if (!(device is AlsaDevice alsaDevice) || alsaDevice == null)
            {
                throw new ArgumentException($"Only {typeof(AlsaDevice).FullName} are supported", nameof(device));
            }

            if(!ManagedAlsaAudioClient.TryInitialize(alsaDevice.GetManagedDevice(), format, out supportedFormat))
            {
                client = null;
                return false;
            }

            client = new AlsaAudioClient(alsaDevice);
            supportedFormat = format;
            return true;
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
