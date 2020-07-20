using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Wasapi.Interop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;

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
