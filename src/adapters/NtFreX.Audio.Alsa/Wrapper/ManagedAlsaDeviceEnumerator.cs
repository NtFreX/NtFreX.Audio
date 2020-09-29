using System;

namespace NtFreX.Audio.Alsa.Wrapper
{
    internal class ManagedAlsaDeviceEnumerator
    {
        public static ManagedAlsaDevice GetDefaultRenderDevice()
        {
            IntPtr devicePtr = IntPtr.Zero;
            var id = "default";
            Interop.Alsa
                .snd_pcm_open(ref devicePtr, id, Interop.snd_pcm_stream_t.SND_PCM_STREAM_PLAYBACK, 0)
                .ThrowIfNotSucceded("Could not get the default render device");
            return new ManagedAlsaDevice(devicePtr, id);
        }

        public static ManagedAlsaDevice GetDefaultCaptureDevice()
        {
            IntPtr devicePtr = IntPtr.Zero;
            var id = "default";
            Interop.Alsa
                .snd_pcm_open(ref devicePtr, id, Interop.snd_pcm_stream_t.SND_PCM_STREAM_CAPTURE, 0)
                .ThrowIfNotSucceded("Could not get the default capture device");
            return new ManagedAlsaDevice(devicePtr, id);
        }
    }
}
