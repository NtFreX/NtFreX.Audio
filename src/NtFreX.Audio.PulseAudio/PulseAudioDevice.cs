using NtFreX.Audio.PulseAudio.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace NtFreX.Audio.PulseAudio
{
    public class PulseAudioDevice
    {
        public PulseAudioPlaybackContext Play(byte[] data)
        {
            var spec = new pa_sample_spec
            {
                channels = 1,
                format = pa_sample_format.PA_SAMPLE_U8,
                rate = 44100
            };

            var error = 0;
            var client = Simple.pa_simple_new(null, "NtFreX.Audio", pa_stream_direction.PA_STREAM_PLAYBACK, null, "Unknown", spec, null, null, ref error);
            if (error != 0) throw new Exception(error.ToString());

            return new PulseAudioPlaybackContext(client, data);
        }
    }

    public class PulseAudioPlaybackContext: IDisposable
    {
        private readonly pa_simple client;
        private readonly Thread thread;

        internal PulseAudioPlaybackContext(pa_simple client, byte[] data)
        {
            this.client = client;
            thread = new Thread(() =>
            {
                PumpAudio(data);
            });
            thread.Start();
        }

        private void PumpAudio(byte[] data)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr dataPtr = handle.AddrOfPinnedObject();

            var error = 0;
            Simple.pa_simple_write(client, dataPtr, (uint) data.Length, ref error);
            if (error != 0) throw new Exception(error.ToString());

            Simple.pa_simple_drain(client, ref error);
            if (error != 0) throw new Exception(error.ToString());
        }

        public void Dispose()
        {
            Simple.pa_simple_free(client);
        }
    }
}
