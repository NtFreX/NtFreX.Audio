using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Wasapi.Wrapper;

namespace NtFreX.Audio.Wasapi
{
    internal static class ManagedWaveFormatExtensions
    {
        public static AudioFormat ToAudioFormat(this ManagedWaveFormat audioFormat)
        {
            return new AudioFormat(
                audioFormat.Unmanaged.Format.SamplesPerSec, 
                audioFormat.Unmanaged.Format.BitsPerSample,
                ChannelFactory.GetChannels(audioFormat.Unmanaged.ChannelMask),
                MediaTypeFactory.GetMediaType(audioFormat.Unmanaged.SubFormat));
        }
    }
}
