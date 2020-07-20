using NtFreX.Audio.AdapterInfrastructure;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Devices
{
    public interface IAudioPlatformAdapter
    {
        bool CanUse();
        [return: NotNull] IAudioPlatform Initialize();
    }
}
