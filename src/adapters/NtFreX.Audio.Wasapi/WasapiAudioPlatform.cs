using NtFreX.Audio.AdapterInfrastructure;

namespace NtFreX.Audio.Wasapi
{
    public sealed class WasapiAudioPlatform : IAudioPlatform
    {
        public static WasapiAudioPlatform Instance { get; } = new WasapiAudioPlatform();

        public IAudioDeviceFactory AudioDeviceFactory { get; } = new MultiMediaDeviceFactory();
    }
}
