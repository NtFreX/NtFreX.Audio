using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Devices.Adapters
{
    internal interface IAudioDeviceAdapter
    {
        bool CanUse();
        [return:NotNull] IAudioDevice Initialize();
    }
}
