using NtFreX.Audio.Infrastructure;
using System;

namespace NtFreX.Audio.Alsa.Wrapper
{
    internal class ManagedAlsaAudioClient
    {
        public static bool TryInitialize(ManagedAlsaDevice device, IAudioFormat format, out IAudioFormat supportedFormat)
        {
            Interop.Alsa.snd_pcm_hw_params_malloc(out var parameters).ThrowIfNotSucceded("Can not allocate parameters object.");

            Interop.Alsa.snd_pcm_hw_params_any(device, parameters).ThrowIfNotSucceded("Can not fill parameters object.");

            Interop.Alsa.snd_pcm_hw_params_set_access(device, parameters, Interop.snd_pcm_access_t.SND_PCM_ACCESS_RW_INTERLEAVED).ThrowIfNotSucceded("Can not set access mode.");

            var result = (format.BitsPerSample / 8) switch
            {
                1 => Interop.Alsa.snd_pcm_hw_params_set_format(device, parameters, Interop.snd_pcm_format_t.SND_PCM_FORMAT_U8),
                2 => Interop.Alsa.snd_pcm_hw_params_set_format(device, parameters, Interop.snd_pcm_format_t.SND_PCM_FORMAT_S16_LE),
                3 => Interop.Alsa.snd_pcm_hw_params_set_format(device, parameters, Interop.snd_pcm_format_t.SND_PCM_FORMAT_S24_LE),
                _ => throw new Exception("Bits per sample error. Please reset the value of RecordingBitsPerSample."),
            };
            result.ThrowIfNotSucceded("Can not set format.");

            Interop.Alsa.snd_pcm_hw_params_set_channels(device, parameters, format.Channels).ThrowIfNotSucceded("Can not set channel.");

            var sampleRate = format.SampleRate;
            Interop.Alsa.snd_pcm_hw_params_set_rate_near(device, parameters, ref sampleRate, out var dir).ThrowIfNotSucceded("Can not set rate.");
            // TODO: find correct supported format
            supportedFormat = new AudioFormat(sampleRate, format.BitsPerSample, format.Channels, format.Type);

            Interop.Alsa.snd_pcm_hw_params(device, parameters).ThrowIfNotSucceded("Can not set hardware parameters.");
            return true;
        }
    }
}
