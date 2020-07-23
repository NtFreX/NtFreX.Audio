using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
using System;

namespace NtFreX.Audio.Wasapi
{
    public class WasapiAudioClientFactory : IAudioClientFactory
    {
        public AudioFormat GetDefaultFormat(IAudioDevice device)
        {
            if (!(device is MultiMediaDevice multiMediaDevice) || multiMediaDevice == null)
            {
                throw new ArgumentException($"Only {typeof(MultiMediaDevice).FullName} are supported", nameof(device));
            }

            //TODO dot not dispose client only to get mix format?
            var wrapper = multiMediaDevice.ToManaged();
            using var audioClient = wrapper.Activate();

            using var mixFormat = audioClient.GetMixFormat();

            return mixFormat?.ToAudioFormat() ?? throw new Exception();
        }

        public bool TryInitialize(IAudioFormat format, IAudioDevice device, out IAudioClient? client, out IAudioFormat supportedFormat)
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
