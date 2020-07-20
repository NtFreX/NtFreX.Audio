using System;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IAudioDevice : IDisposable
    {
        string GetId();
    }
}
