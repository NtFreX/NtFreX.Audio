namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IAudioClientFactory
    {
        bool TryInitialize(AudioFormat format, IAudioDevice device, out IAudioClient? client, out AudioFormat supportedFormat);
    }
}
