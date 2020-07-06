using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NtFreX.Audio.Devices.Adapters
{
    public sealed class AudioDeviceAdapterFactory
    {
        private readonly IAudioDeviceAdapter[] _audioDeviceResolvers = new[]
        {
            new WindowsFileSystemAudioDeviceAdapter()
        };

        public static AudioDeviceAdapterFactory Instance { [return:NotNull] get; } = new AudioDeviceAdapterFactory();

        private AudioDeviceAdapterFactory() { }

        [return: NotNull] public IAudioDevice Get()
        {
            var audioFileResolver = _audioDeviceResolvers.FirstOrDefault(x => x.CanUse());
            if (audioFileResolver == null)
            {
                throw new PlatformNotSupportedException("No audio device handler for the current platform has been found.");
            }

            return audioFileResolver.Initialize();
        }
    }
}
