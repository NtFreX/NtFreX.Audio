using System;

namespace NtFreX.Audio.Alsa.Wrapper
{
    internal class ManagedAlsaDeviceEnumerator
    {
        public static ManagedAlsaDevice GetDefaultRenderDevice()
        {
            IntPtr devicePtr = IntPtr.Zero;
            var id = "default";
            var error = Interop.snd_pcm_open(ref devicePtr, id, snd_pcm_stream_t.SND_PCM_STREAM_PLAYBACK, 0);
            return new ManagedAlsaDevice(devicePtr, id);
        }

        public static ManagedAlsaDevice GetDefaultCaptureDevice()
        {
            IntPtr devicePtr = IntPtr.Zero;
            var id = "default";
            var error = Interop.snd_pcm_open(ref devicePtr, id, snd_pcm_stream_t.SND_PCM_STREAM_CAPTURE, 0);
            return new ManagedAlsaDevice(devicePtr, id);
        }
    }
}
