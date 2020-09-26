using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
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
    internal class ManagedAudioRender : IAsyncDisposable
    {
        public const int RefimesPerSec = 10000000;
        public const int RefTimesPerMilisec = 10000;

        private const int EventDelay = 100;

        private readonly ManagedAudioClient managedAudioClient;
        private readonly ManagedAudioClock managedAudioClock;
        private readonly IAudioRenderClient audioRenderClient;
        private readonly CancellationToken cancellationToken;
        private readonly IAudioContainer audio;
        private readonly ISeekableAsyncEnumerator<IReadOnlyList<byte>> enumerator;
        private readonly Task audioPump;
        private readonly Task eventPump;
        private readonly uint bufferFrameCount;

        private bool isDisposed;

        public Observable<EventArgs<Exception>> RenderExceptionOccured { get; } = new Observable<EventArgs<Exception>>();
        public Observable<EventArgs> EndOfDataReached { get; } = new Observable<EventArgs>();
        public Observable<EventArgs> EndOfPositionReached { get; } = new Observable<EventArgs>();
        public Observable<EventArgs<double>> PositionChanged { get; } = new Observable<EventArgs<double>>();
        public Observable<EventArgs> RenderCanceled { get; } = new Observable<EventArgs>();

        internal ManagedAudioRender(ManagedAudioClient managedAudioClient, ManagedAudioClock managedAudioClock, IAudioRenderClient audioRenderClient, IAudioContainer audio, CancellationToken cancellationToken)
        {
            this.enumerator = audio.GetAsyncAudioEnumerator(cancellationToken);
            this.bufferFrameCount = managedAudioClient.GetBufferSize();
            this.managedAudioClient = managedAudioClient;
            this.managedAudioClock = managedAudioClock;
            this.audioRenderClient = audioRenderClient;
            this.cancellationToken = cancellationToken;
            this.audio = audio;

            var taskSchedulerPair = new ConcurrentExclusiveSchedulerPair();
            eventPump = Task.Factory.StartNew(PumpEventsAsync, cancellationToken, TaskCreationOptions.DenyChildAttach, taskSchedulerPair.ConcurrentScheduler).Unwrap();
            audioPump = Task.Factory.StartNew(PumpAudioAsync, cancellationToken, TaskCreationOptions.DenyChildAttach, taskSchedulerPair.ConcurrentScheduler).Unwrap();
        }

        public async ValueTask DisposeAsync()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;

            await audioPump.IgnoreCancelationError().ConfigureAwait(false);
            await eventPump.IgnoreCancelationError().ConfigureAwait(false);

            managedAudioClock.Dispose();
            managedAudioClient.Stop();
            Marshal.ReleaseComObject(audioRenderClient);

            audioPump.Dispose();
            eventPump.Dispose();

            RenderExceptionOccured.Dispose();
            EndOfDataReached.Dispose();
            EndOfPositionReached.Dispose();
            PositionChanged.Dispose();
            RenderCanceled.Dispose();

            await enumerator.DisposeAsync().ConfigureAwait(false);
        }

        public void Stop() => managedAudioClient.Stop();
        public void Start() => managedAudioClient.Start();

        public TimeSpan GetPosition()
        {
            // TODO: use enumerator.position? what to do with managedAudioClock
            // TimeSpan.FromSeconds(managedAudioClock.GetPosition());
            var positionInBuffer = enumerator.GetPosition();
            var factor = positionInBuffer / enumerator.GetDataLength();
            return audio.GetLength() * factor;
        }
        public void SetPosition(TimeSpan position)
        {
            var format = audio.GetFormat();
            var totalInBytes = audio.GetByteLength();
            var positionInBytes = position.TotalSeconds * format.SampleRate * format.Channels * format.BytesPerSample;
            var factor = positionInBytes / totalInBytes;
            var positionInBuffer = enumerator.GetDataLength() * factor;
            enumerator.SeekTo((long) positionInBuffer);
        }

        private async Task PumpEventsAsync()
        {
            var totalLength = audio.GetLength().TotalSeconds;
            while(!isDisposed && !cancellationToken.IsCancellationRequested)
            {
                var position = managedAudioClock.GetPosition();
                await PositionChanged.InvokeAsync(this, new EventArgs<double>(position)).ConfigureAwait(false);
                
                // TODO: find out why pos bigger total pos
                if(position >= totalLength)
                {
                    await EndOfPositionReached.InvokeAsync(this, EventArgs.Empty).ConfigureAwait(false);
                }

                await Task.Delay(EventDelay, cancellationToken).ConfigureAwait(false);
            }
        }
        private async Task PumpAudioAsync()
        {
            try
            {
                if (managedAudioClient.InitializedFormat == null)
                {
                    throw new Exception("The audio client has no intialized format");
                }

                var format = managedAudioClient.InitializedFormat.Unmanaged;
                var hasStarted = false;
                var realBuffer = new List<byte>();
                var hnsActualDuration = (double)RefimesPerSec * bufferFrameCount / format.Format.SamplesPerSec;
                while(await enumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }

                    if (enumerator.Current != null)
                    {
                        realBuffer.AddRange(enumerator.Current);
                    }

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

                // Wait for last data in buffer to play before stopping.
                await Task.Delay((int)(hnsActualDuration / RefTimesPerMilisec / 2), cancellationToken).ConfigureAwait(false);

                await EndOfDataReached.InvokeAsync(this, EventArgs.Empty).ConfigureAwait(false);
            }
            catch(OperationCanceledException)
            {
                await RenderCanceled.InvokeAsync(this, EventArgs.Empty).ConfigureAwait(false);
            }
            catch(Exception exce)
            {
                await RenderExceptionOccured.InvokeAsync(this, new EventArgs<Exception>(exce)).ConfigureAwait(false);
            }
        }

        private void RenderFrames(List<byte> realBuffer, uint numFramesAvailable)
        {
            if (managedAudioClient.InitializedFormat == null)
            {
                throw new Exception("The audio client has no intialized format");
            }

            // To ensure the correct ordering of calls, a GetBuffer call and its corresponding ReleaseBuffer call must occur in the same thread.
            audioRenderClient
                .GetBuffer(numFramesAvailable, out IntPtr dataBufferPointer)
                .ThrowIfNotSucceded("Could not get the data buffer");
            
            if (dataBufferPointer == IntPtr.Zero)
            {
                // when audio stream is paused for a longer time we may end up here
                return;
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
