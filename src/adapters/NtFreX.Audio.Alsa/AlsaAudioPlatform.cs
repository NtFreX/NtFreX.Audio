using NtFreX.Audio.AdapterInfrastructure;

namespace NtFreX.Audio.Alsa
{
    public sealed class AlsaAudioPlatform : IAudioPlatform
    {
        public IAudioDeviceFactory AudioDeviceFactory { get; } = new AlsaDeviceFactory();

        public IAudioClientFactory AudioClientFactory { get; } = new AlsaAudioClientFactory();
    }
}
