using NtFreX.Audio.AdapterInfrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Devices
{
    internal class WasapiAudioPlatformAdapter : IAudioPlatformAdapter
    {
        public bool CanUse()
            => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        [return: NotNull]
        public IAudioPlatform Initialize()
            => AsssemblyAudioDeviceLoader.Initialize("NtFreX.Audio.Wasapi", "WasapiAudioPlatform");
    }
}
