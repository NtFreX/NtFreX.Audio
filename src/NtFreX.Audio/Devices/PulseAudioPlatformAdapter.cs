﻿using NtFreX.Audio.AdapterInfrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Devices
{
    internal class PulseAudioPlatformAdapter : IAudioPlatformAdapter
    {
        public bool CanUse()
            => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        [return: NotNull]
        public IAudioPlatform Initialize()
            => AsssemblyAudioDeviceLoader.Initialize("NtFreX.Audio.PulseAudio", "PulseAudioPlatform");
    }
}
