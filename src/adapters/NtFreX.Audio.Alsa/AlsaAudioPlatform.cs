using NtFreX.Audio.AdapterInfrastructure;

namespace NtFreX.Audio.Alsa
{
    public sealed class AlsaAudioPlatform : IAudioPlatform
    {
        public static AlsaAudioPlatform Instance { get; } = new AlsaAudioPlatform();

        public IAudioDeviceFactory AudioDeviceFactory { get; } = new AlsaDeviceFactory();
    }
}
