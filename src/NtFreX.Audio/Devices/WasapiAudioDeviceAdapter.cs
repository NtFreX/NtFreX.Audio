using NtFreX.Audio.AdapterInfrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Devices
{
    internal class WasapiAudioDeviceAdapter : IAudioDeviceAdapter
    {
        public bool CanUse()
            => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        [return: NotNull]
        public IAudioDevice Initialize()
            => AsssemblyAudioDeviceLoader.Initialize("NtFreX.Audio.Wasapi", "WasapiAudioDevice");
    }
}
