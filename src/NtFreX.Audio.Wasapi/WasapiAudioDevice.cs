using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Wasapi.Interop;
using NtFreX.Audio.Wasapi.Wrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Wasapi
{
    public class WasapiAudioDevice : IAudioDevice
    {
        private AudioClient? audioClient;
        private IWaveAudioContainer? audio;

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/coreaudio/rendering-a-stream
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [return: NotNull]
        public Task<IPlaybackContext> PlayAsync(CancellationToken cancellationToken = default)
        {
            if(audio == null || audioClient == null)
            {
                throw new Exception("The device is not initialized");
            }
            // todo: dispose/cleanup
            var context = new WasapiPlaybackContext(audio, audioClient, cancellationToken);
            return Task.FromResult(context as IPlaybackContext);
        }

        public bool TryInitialize(IWaveAudioContainer audio, out Format supportedFormat)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var device = MultiMediaDeviceEnumerator.Instance.GetDefaultRenderDevice();
            this.audioClient = device.TryGetAudioClient(audio, out var supportedFormatInner);
            this.audio = audio;

            supportedFormat = new Format(supportedFormatInner.Format.SamplesPerSec, supportedFormatInner.Format.BitsPerSample);
            return IsInitialized();
        }

        public bool IsInitialized() => audioClient != null;

        public void Dispose()
        {
            //TODO: implement
        }
    }

    public class WasapiPlaybackContext : IPlaybackContext
    {
        public const int REFTIMES_PER_SEC = 10000000;
        public const int REFTIMES_PER_MILLISEC = 10000;

        private readonly IWaveAudioContainer audio;
        private readonly IAudioClient audioClient;
        private readonly IAudioRenderClient audioRenderClient;
        private readonly WaveFormatDefinition format;
        private readonly CancellationToken cancellationToken;
        private readonly uint bufferFrameCount;
        private readonly Task task;

        public Observable<EventArgs> EndOfDataReached { get; } = new Observable<EventArgs>();

        internal WasapiPlaybackContext(IWaveAudioContainer audio, AudioClient audioClient, CancellationToken cancellationToken)
        {
            this.audio = audio;
            this.audioClient = audioClient.audioClient;
            this.format = audioClient.format;
            this.cancellationToken = cancellationToken;

            var getServiceResult = this.audioClient.GetService(new Guid(ClsId.IAudioRendererClient), out object audioRenderClientObj);
            if (getServiceResult != HResult.S_OK || !(audioRenderClientObj is IAudioRenderClient audioRenderClient) || audioRenderClient == null)
            {
                throw new Exception("Could not get the audio renderer client", new COMException(null, (int)getServiceResult));
            }
            this.audioRenderClient = audioRenderClient;

            if (this.audioClient.GetBufferSize(out uint bufferFrameCount) != HResult.S_OK)
            {
                throw new Exception("Could not get the buffer size");
            }
            this.bufferFrameCount = bufferFrameCount;

            task = Task.Run(PumpAudioAsync, cancellationToken);
        }

        private async Task PumpAudioAsync()
        {
            var hasStarted = false;
            var realBuffer = new List<byte>();
            var hnsActualDuration = (double)REFTIMES_PER_SEC *
                        bufferFrameCount / format.Format.SamplesPerSec;
            await foreach (var buffer in audio.GetAudioSamplesAsync(cancellationToken).ConfigureAwait(false).WithCancellation(cancellationToken))
            {
                realBuffer.AddRange(buffer);

                while (realBuffer.Count >= bufferFrameCount * format.Format.BlockAlign)
                {
                    if (!hasStarted)
                    {
                        // Fill first buffer before audio client has started
                        RenderFrames(realBuffer, bufferFrameCount);
                        if (audioClient.Start() != HResult.S_OK)
                        {
                            throw new Exception("Could not start the audio client");
                        }
                        hasStarted = true;
                    }
                    else
                    {
                        // Fill buffer when audio client request new data
                        await Task.Delay((int)(hnsActualDuration / REFTIMES_PER_MILLISEC / 2)).ConfigureAwait(false);

                        // See how much buffer space is available.
                        if (audioClient.GetCurrentPadding(out uint numFramesPadding) != HResult.S_OK)
                        {
                            throw new Exception("Could not get the current padding");
                        }

                        var numFramesAvailable = bufferFrameCount - numFramesPadding;
                        RenderFrames(realBuffer, numFramesAvailable);
                    }
                }
            }

            EndOfDataReached?.Invoke(this, EventArgs.Empty);
        }

        private void RenderFrames(List<byte> realBuffer, uint numFramesAvailable)
        {
            // To ensure the correct ordering of calls, a GetBuffer call and its corresponding ReleaseBuffer call must occur in the same thread.
            if (audioRenderClient.GetBuffer(numFramesAvailable, out IntPtr dataBufferPointer) != HResult.S_OK || dataBufferPointer == IntPtr.Zero)
            {
                throw new Exception("Could not get the data buffer");
            }

            var realSize = numFramesAvailable * format.Format.BlockAlign;
            Marshal.Copy(realBuffer.ToArray(), 0, dataBufferPointer, (int)realSize);
            realBuffer.RemoveRange(0, (int)realSize);

            if (audioRenderClient.ReleaseBuffer(numFramesAvailable, AudioClientBufferFlags.None) != HResult.S_OK)
            {
                throw new Exception("Could not release the buffer");
            }
        }

        public void Dispose()
        {
            //TODO: cleanup all the stuff
            //task.Dispose();
            if (audioClient.Stop() != HResult.S_OK)
            {
                throw new Exception("Could not stop the audio client");
            }
        }
    }
}
