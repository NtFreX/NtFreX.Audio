using NtFreX.Audio.Wasapi.Interop;
using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Wrapper
{
    internal class ManagedAudioClock : IDisposable
    {
        private readonly IAudioClock audioClock;

        internal ManagedAudioClock(IAudioClock audioClock)
        {
            this.audioClock = audioClock;
        }

        public double GetPosition()
        {
            audioClock.GetPosition(out var pos, out _).ThrowIfNotSucceded();
            audioClock.GetFrequency(out var freq).ThrowIfNotSucceded();

            return pos / (double) freq;
        }

        public void Dispose()
        {
            Marshal.ReleaseComObject(audioClock);
        }
    }
}
