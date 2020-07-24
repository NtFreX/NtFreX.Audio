using NtFreX.Audio.AdapterInfrastructure;

namespace NtFreX.Audio.Wasapi
{
    public sealed class WasapiAudioPlatform : IAudioPlatform
    {
        public IAudioDeviceFactory AudioDeviceFactory { get; } = new MultiMediaDeviceFactory();

        public IAudioClientFactory AudioClientFactory { get; } = new WasapiAudioClientFactory();
    }
}
