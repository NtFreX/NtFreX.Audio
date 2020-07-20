using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Wasapi.Wrapper;

namespace NtFreX.Audio.Wasapi
{
    internal static class AudioFormatExtensions
    {
        public static ManagedWaveFormat ToManagedWaveFormat(this AudioFormat audioFormat)
        {
            var format = new Interop.WaveFormatDefinition(
                audioFormat.Channels,
                audioFormat.SampleRate,
                audioFormat.BitsPerSample,
                ChannelFactory.GetDefaultMapping(audioFormat.Channels),
                MediaTypeFactory.GetMediaType(audioFormat.Type));

            return new ManagedWaveFormat(format);
        }
    }
}
