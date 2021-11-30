using NtFreX.Audio.Infrastructure;
using System;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IAudioDevice : IDisposable
    {
        string GetId();
        AudioFormat GetDefaultFormat();
        bool TryInitialize(IAudioFormat format, out IAudioClient? client, out IAudioFormat supportedFormat);
    }
}
