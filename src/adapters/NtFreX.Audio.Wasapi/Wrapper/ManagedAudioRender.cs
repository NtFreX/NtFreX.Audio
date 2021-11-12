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
        // TODO: what to do with ManagedAudioClock?
        private readonly ManagedAudioClock managedAudioClock;
        private readonly IAudioRenderClient audioRenderClient;
        private readonly CancellationToken cancellationToken;
        private readonly IAudioContainer audio;
        private readonly ISeekableAsyncEnumerator<Memory<byte>> enumerator;
        private readonly uint bufferFrameCount;
        private readonly ConcurrentExclusiveSchedulerPair taskSchedulerPair;
        private readonly List<byte> realBuffer;

        private Task? audioPump;
        private Task? eventPump;
        private bool isStopped;
        private bool isDisposed;

        public Observable<EventArgs<Exception>> RenderExceptionOccured { get; } = new Observable<EventArgs<Exception>>();
        public Observable<EventArgs> EndOfDataReached { get; } = new Observable<EventArgs>();
        public Observable<EventArgs> EndOfPositionReached { get; } = new Observable<EventArgs>();
        public Observable<EventArgs<double>> PositionChanged { get; } = new Observable<EventArgs<double>>();
        public Observable<EventArgs> RenderCanceled { get; } = new Observable<EventArgs>();

        internal ManagedAudioRender(ManagedAudioClient managedAudioClient, ManagedAudioClock managedAudioClock, IAudioRenderClient audioRenderClient, IAudioContainer audio, CancellationToken cancellationToken)
        {
            this.enumerator = audio.GetAsyncAudioEnumerable(cancellationToken).GetAsyncEnumerator(cancellationToken);
            this.bufferFrameCount = managedAudioClient.GetBufferSize();
            this.managedAudioClient = managedAudioClient;
            this.managedAudioClock = managedAudioClock;
            this.audioRenderClient = audioRenderClient;
            this.cancellationToken = cancellationToken;
            this.audio = audio;
            this.isStopped = false;
            this.realBuffer = new List<byte>();

            taskSchedulerPair = new ConcurrentExclusiveSchedulerPair();
            StartEventPump();
            StartAudioPump();
        }

        public async ValueTask DisposeAsync()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;

            if (audioPump != null)
            {
                await audioPump.IgnoreCancelationError().ConfigureAwait(false);
                audioPump.Dispose();
            }
            if (eventPump != null)
            {
                await eventPump.IgnoreCancelationError().ConfigureAwait(false);
                eventPump.Dispose();
            }

            managedAudioClock.Dispose();
            if (!isStopped)
            {
                managedAudioClient.Stop();
            }
            Marshal.ReleaseComObject(audioRenderClient);

            RenderExceptionOccured.Dispose();
            EndOfDataReached.Dispose();
            EndOfPositionReached.Dispose();
            PositionChanged.Dispose();
            RenderCanceled.Dispose();

            await enumerator.DisposeAsync().ConfigureAwait(false);
        }

        public bool IsStopped() => isStopped;
        public void Stop()
        {
            isStopped = true;
            managedAudioClient.Stop();
        }
        public void Start()
        {
            isStopped = false;
            managedAudioClient.Start();
        }

        public TimeSpan GetLength() => audio.GetLength();
        public TimeSpan GetPosition()
        {
            // TODO: use enumerator.position? what to do with managedAudioClock
            // TimeSpan.FromSeconds(managedAudioClock.GetPosition());
            // differentiate between filled buffer position and rendered position
            var positionInBuffer = enumerator.GetPosition();
            var factor = 1.0f * positionInBuffer / enumerator.GetDataLength();
            return audio.GetLength() * factor;
        }
        public void SetPosition(TimeSpan position)
        {
            if (position.TotalSeconds < 0)
            {
                position = TimeSpan.FromSeconds(0);
            }

            var format = audio.GetFormat();
            var totalInBytes = audio.GetByteLength();
            var positionInBytes = position.TotalSeconds * format.SampleRate * format.Channels * format.BytesPerSample;
            var factor = positionInBytes / totalInBytes;
            var lengthInBuffer = enumerator.GetDataLength();
            var positionInBuffer = (ulong) (lengthInBuffer * factor);

            if(positionInBuffer > lengthInBuffer)
            {
                positionInBuffer = lengthInBuffer;
            }

            enumerator.SeekTo((long) positionInBuffer);
            realBuffer.Clear();

            if (audioPump?.Status == TaskStatus.RanToCompletion)
            {
                StartAudioPump();
            }
            if(eventPump?.Status == TaskStatus.RanToCompletion)
            {
                StartEventPump();
            }
        }

        private async Task PumpEventsAsync()
        {
            var totalLength = audio.GetLength().TotalSeconds;
            while(!isDisposed && !cancellationToken.IsCancellationRequested)
            {
                var position = GetPosition().TotalSeconds;
                await PositionChanged.InvokeAsync(this, new EventArgs<double>(position)).ConfigureAwait(false);
                
                // TODO: is the position bigger then the total length because the last buffer may not be full?
                if(position >= totalLength)
                {
                    await EndOfPositionReached.InvokeAsync(this, EventArgs.Empty).ConfigureAwait(false);
                    break;
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
                var hnsActualDuration = (double)RefimesPerSec * bufferFrameCount / format.Format.SamplesPerSec;
                var hasAudio = true;
                while(!isDisposed && hasAudio)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }

                    if (!isStopped)
                    {
                        hasAudio = await enumerator.MoveNextAsync().ConfigureAwait(false);
                        realBuffer.AddRange(enumerator.Current.ToArray());
                    }

                    while (!isDisposed && realBuffer.Count >= bufferFrameCount * format.Format.BlockAlign)
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

                while (!isDisposed && realBuffer.Count > 0)
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

        private void StartEventPump()
        {
            eventPump?.Dispose();
            eventPump = Task.Factory.StartNew(PumpEventsAsync, cancellationToken, TaskCreationOptions.DenyChildAttach, taskSchedulerPair.ConcurrentScheduler).Unwrap();
        }

        private void StartAudioPump()
        {
            audioPump?.Dispose();
            audioPump = Task.Factory.StartNew(PumpAudioAsync, cancellationToken, TaskCreationOptions.DenyChildAttach, taskSchedulerPair.ConcurrentScheduler).Unwrap();
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
