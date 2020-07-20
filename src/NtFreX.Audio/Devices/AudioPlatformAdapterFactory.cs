using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Resources;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NtFreX.Audio.Devices.Adapters
{
    public sealed class AudioPlatformAdapterFactory
    {
        private readonly IAudioPlatformAdapter[] audioDeviceResolvers = new IAudioPlatformAdapter[]
        {
            new WasapiAudioPlatformAdapter(),
            new PulseAudioPlatformAdapter()
        };

        public static AudioPlatformAdapterFactory Instance { [return:NotNull] get; } = new AudioPlatformAdapterFactory();

        private AudioPlatformAdapterFactory() { }

        [return: NotNull] public IAudioPlatform Get()
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
