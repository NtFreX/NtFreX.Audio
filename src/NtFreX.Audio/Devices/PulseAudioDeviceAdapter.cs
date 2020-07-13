using NtFreX.Audio.AdapterInfrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Devices
{
    internal class PulseAudioDeviceAdapter : IAudioDeviceAdapter
    {
        public bool CanUse()
            => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        [return: NotNull]
        public IAudioDevice Initialize()
            => AsssemblyAudioDeviceLoader.Initialize("NtFreX.Audio.PulseAudio", "PulseAudioDevice");
    }
}
