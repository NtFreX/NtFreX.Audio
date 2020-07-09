using NtFreX.Audio.Resources;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NtFreX.Audio.Devices.Adapters
{
    public sealed class AudioDeviceAdapterFactory
    {
        private readonly IAudioDeviceAdapter[] audioDeviceResolvers = new[]
        {
            new WindowsFileSystemAudioDeviceAdapter()
        };

        public static AudioDeviceAdapterFactory Instance { [return:NotNull] get; } = new AudioDeviceAdapterFactory();

        private AudioDeviceAdapterFactory() { }

        [return: NotNull] public IAudioDevice Get()
        {
            var audioFileResolver = audioDeviceResolvers.FirstOrDefault(x => x.CanUse());
            if (audioFileResolver == null)
            {
                throw new PlatformNotSupportedException(ExceptionMessages.NoAudioDeviceForPlatform);
            }

            return audioFileResolver.Initialize();
        }
    }
}
