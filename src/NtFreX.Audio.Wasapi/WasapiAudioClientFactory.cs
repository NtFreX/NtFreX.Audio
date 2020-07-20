using NtFreX.Audio.AdapterInfrastructure;
using System;

namespace NtFreX.Audio.Wasapi
{
    public class WasapiAudioClientFactory : IAudioClientFactory
    {
        public bool TryInitialize(AudioFormat format, IAudioDevice device, out IAudioClient? client, out AudioFormat supportedFormat)
        {
            _ = format ?? throw new ArgumentNullException(nameof(format));

            if (!(device is MultiMediaDevice multiMediaDevice) || multiMediaDevice == null)
            {
                throw new ArgumentException($"Only {typeof(MultiMediaDevice).FullName} are supported", nameof(device));
            }

            var wrapper = multiMediaDevice.ToManaged();
            var managedFormat = format.ToManagedWaveFormat();

            var audioClient = wrapper.Activate();
            var success = audioClient.TryInitialize(managedFormat, out var managedSupportedFormat);

            supportedFormat = managedSupportedFormat.ToAudioFormat();
            managedSupportedFormat.Dispose();

            client = success ? new WasapiAudioClient(audioClient) : null;
            return success;
        }
    }
}
