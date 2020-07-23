using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Wasapi.Interop;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Wasapi.Wrapper
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/coreaudio/rendering-a-stream
    /// </summary>
    internal class ManagedAudioRender : IDisposable
    {
        public const int RefimesPerSec = 10000000;
        public const int RefTimesPerMilisec = 10000;

        private const int EventDelay = 100;

        private readonly ManagedAudioClient managedAudioClient;
        private readonly ManagedAudioClock managedAudioClock;
        private readonly IAudioRenderClient audioRenderClient;
        private readonly CancellationToken cancellationToken;
        private readonly IWaveAudioContainer audio;
        private readonly Task audioPump;
        private readonly Task eventPump;
        private readonly uint bufferFrameCount;

        private bool isDisposed = false;

        public Observable<EventArgs> EndOfDataReached { get; } = new Observable<EventArgs>();

        public Observable<EventArgs> EndOfPositionReached { get; } = new Observable<EventArgs>();

        public Observable<EventArgs<double>> PositionChanged { get; } = new Observable<EventArgs<double>>();

        internal ManagedAudioRender(ManagedAudioClient managedAudioClient, ManagedAudioClock managedAudioClock, IAudioRenderClient audioRenderClient, IWaveAudioContainer audio, CancellationToken cancellationToken)
        {
            this.bufferFrameCount = managedAudioClient.GetBufferSize();
            this.managedAudioClient = managedAudioClient;
            this.managedAudioClock = managedAudioClock;
            this.audioRenderClient = audioRenderClient;
            this.cancellationToken = cancellationToken;
            this.audio = audio;

            eventPump = Task.Run(PumpEventsAsync, cancellationToken);
            audioPump = Task.Run(PumpAudioAsync, cancellationToken);
        }

        public void Dispose()
        {
            //TODO: cleanup all the stuff
            //task.Dispose();
            managedAudioClock.Dispose();
            managedAudioClient.Stop();
            Marshal.ReleaseComObject(audioRenderClient);
            _ = audioPump.ContinueWith(x => x.Dispose(), TaskScheduler.Default);
            _ = eventPump.ContinueWith(x => x.Dispose(), TaskScheduler.Default);
            isDisposed = true;
        }

        private async Task PumpEventsAsync()
        {
            var totalLength = audio.GetLength().TotalSeconds;
            while(!isDisposed)
            {
                var position = managedAudioClock.GetPosition();
                PositionChanged?.Invoke(this, new EventArgs<double>(position));
                
                // TODO: find out why pos bigger total pos
                if(position >= totalLength)
                {
                    EndOfPositionReached?.Invoke(this, EventArgs.Empty);
                }

                await Task.Delay(EventDelay, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task PumpAudioAsync()
        {
            if(managedAudioClient.InitializedFormat == null)
            {
                throw new Exception("The audio client has no intialized format");
            }

            var format = managedAudioClient.InitializedFormat.Unmanaged;
            var hasStarted = false;
            var realBuffer = new List<byte>();
            var hnsActualDuration = (double)RefimesPerSec * bufferFrameCount / format.Format.SamplesPerSec;
            await foreach (var buffer in audio.GetAudioSamplesAsync(cancellationToken).ConfigureAwait(false).WithCancellation(cancellationToken))
            {
                realBuffer.AddRange(buffer.AsByteArray());

                while (realBuffer.Count >= bufferFrameCount * format.Format.BlockAlign)
                {
                    if (!hasStarted)
                    {
                        // Fill first buffer before audio client has started
                        RenderFrames(realBuffer, bufferFrameCount);
                        managedAudioClient.Start();
                        hasStarted = true;
                    }
                    else
                    {
                        // Fill buffer when audio client request new data
                        await Task.Delay((int)(hnsActualDuration / RefTimesPerMilisec / 2), cancellationToken).ConfigureAwait(false);

                        // See how much buffer space is available.
                        var numFramesPadding = managedAudioClient.GetCurrentPadding();
                        var numFramesAvailable = bufferFrameCount - numFramesPadding;
                        RenderFrames(realBuffer, numFramesAvailable);
                    }
                }
            }

            while (realBuffer.Count > 0)
            {
                // Fill buffer when audio client request new data
                await Task.Delay((int)(hnsActualDuration / RefTimesPerMilisec / 2), cancellationToken).ConfigureAwait(false);

                // See how much buffer space is available.
                var numFramesPadding = managedAudioClient.GetCurrentPadding();
                var numFramesAvailable = bufferFrameCount - numFramesPadding;
                RenderFrames(realBuffer, numFramesAvailable);
            }

            EndOfDataReached?.Invoke(this, EventArgs.Empty);
        }

        private void RenderFrames(List<byte> realBuffer, uint numFramesAvailable)
        {
            if (managedAudioClient.InitializedFormat == null)
            {
                throw new Exception("The audio client has no intialized format");
            }

            // To ensure the correct ordering of calls, a GetBuffer call and its corresponding ReleaseBuffer call must occur in the same thread.
            var error = "Could not get the data buffer";
            audioRenderClient
                .GetBuffer(numFramesAvailable, out IntPtr dataBufferPointer)
                .ThrowIfNotSucceded(error);
            
            if (dataBufferPointer == IntPtr.Zero)
            {
                throw new Exception(error);
            }

            var realSize = Math.Min(numFramesAvailable * managedAudioClient.InitializedFormat.Unmanaged.Format.BlockAlign, realBuffer.Count);
            Marshal.Copy(realBuffer.ToArray(), 0, dataBufferPointer, (int)realSize);
            realBuffer.RemoveRange(0, (int)realSize);

            audioRenderClient
                .ReleaseBuffer(numFramesAvailable, AudioClientBufferFlags.None)
                .ThrowIfNotSucceded("Could not release the buffer");
        }
    }
}
