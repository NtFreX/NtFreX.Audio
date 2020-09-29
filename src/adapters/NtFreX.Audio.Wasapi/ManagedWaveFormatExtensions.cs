using NtFreX.Audio.Infrastructure;
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
                audioFormat.Unmanaged.Format.Channels,
                audioFormat.Unmanaged.Format.FormatTag == Interop.WaveFormatType.EXTENSIBLE ? MediaTypeFactory.GetMediaType(audioFormat.Unmanaged.SubFormat) : (AudioFormatType) audioFormat.Unmanaged.Format.FormatTag);
        }
    }
}
