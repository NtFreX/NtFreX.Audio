﻿using System.IO;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Devices.Adapters
{
    internal sealed class WindowsFileSystemAudioDeviceAdapter : IAudioDeviceAdapter
    {
        public bool CanUse()
            => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && File.Exists(WindowsFileSystemAudioDevice.WindowsMediaPlayerPath);

        public IAudioDevice Initialize()
            => new WindowsFileSystemAudioDevice();
    }
}
