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
        public const int REFTIMES_PER_SEC = 10000000;
        public const int REFTIMES_PER_MILLISEC = 10000;

        private readonly ManagedAudioClient managedAudioClient;
        private readonly IAudioRenderClient audioRenderClient;
        private readonly CancellationToken cancellationToken;
        private readonly IWaveAudioContainer audio;
        private readonly Task task;
        private readonly uint bufferFrameCount;
        public Observable<EventArgs> EndOfDataReached { get; } = new Observable<EventArgs>();

        internal ManagedAudioRender(ManagedAudioClient managedAudioClient, IAudioRenderClient audioRenderClient, IWaveAudioContainer audio, CancellationToken cancellationToken)
        {
            this.bufferFrameCount = managedAudioClient.GetBufferSize();
            this.managedAudioClient = managedAudioClient;
            this.audioRenderClient = audioRenderClient;
            this.cancellationToken = cancellationToken;
            this.audio = audio;

            task = Task.Run(PumpAudioAsync, cancellationToken);
        }

        public void Dispose()
        {
            //TODO: cleanup all the stuff
            //task.Dispose();
            managedAudioClient.Stop();
            Marshal.ReleaseComObject(audioRenderClient);
            _ = task.ContinueWith(x => x.Dispose(), TaskScheduler.Default);
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
            var hnsActualDuration = (double)REFTIMES_PER_SEC * bufferFrameCount / format.Format.SamplesPerSec;
            await foreach (var buffer in audio.GetAudioSamplesAsync(cancellationToken).ConfigureAwait(false).WithCancellation(cancellationToken))
            {
                realBuffer.AddRange(buffer);

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
                        await Task.Delay((int)(hnsActualDuration / REFTIMES_PER_MILLISEC / 2)).ConfigureAwait(false);

                        // See how much buffer space is available.
                        var numFramesPadding = managedAudioClient.GetCurrentPadding();
                        var numFramesAvailable = bufferFrameCount - numFramesPadding;
                        RenderFrames(realBuffer, numFramesAvailable);
                    }
                }
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

            var realSize = numFramesAvailable * managedAudioClient.InitializedFormat.Unmanaged.Format.BlockAlign;
            Marshal.Copy(realBuffer.ToArray(), 0, dataBufferPointer, (int)realSize);
            realBuffer.RemoveRange(0, (int)realSize);

            audioRenderClient
                .ReleaseBuffer(numFramesAvailable, AudioClientBufferFlags.None)
                .ThrowIfNotSucceded("Could not release the buffer");
        }
    }
}
