using NtFreX.Audio.Infrastructure;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IAudioClientFactory
    {
        AudioFormat GetDefaultFormat(IAudioDevice device);
        bool TryInitialize(IAudioFormat format, IAudioDevice device, out IAudioClient? client, out IAudioFormat supportedFormat);
    }
}
