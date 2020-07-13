using NtFreX.Audio.AdapterInfrastructure;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Devices
{
    public interface IAudioDeviceAdapter
    {
        bool CanUse();
        [return: NotNull] IAudioDevice Initialize();
    }
}
