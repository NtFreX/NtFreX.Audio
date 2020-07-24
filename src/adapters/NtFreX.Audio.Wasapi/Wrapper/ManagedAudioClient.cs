using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Wasapi.Interop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;

namespace NtFreX.Audio.Wasapi.Wrapper
{
    internal class ManagedAudioClient : IDisposable
    {
        private readonly IAudioClient audioClient;
        
        public ManagedWaveFormat? InitializedFormat { get; private set; }

        internal ManagedAudioClient(IAudioClient audioClient)
        {
            this.audioClient = audioClient;
        }

        public ManagedWaveFormat GetMixFormat()
        {
            audioClient.GetMixFormat(out var formatPtr).ThrowIfNotSucceded();
            return new ManagedWaveFormat(formatPtr);
        }

        public ManagedAudioCapture GetAudioCapture(AdapterInfrastructure.IAudioSink sink, CancellationToken cancellationToken)
        {
            var error = "Could not get the audio capturer client";
            audioClient.GetService(new Guid(ClsId.IAudioCaptureClient), out object audioCaptureClientObj).ThrowIfNotSucceded(error);
            if (!(audioCaptureClientObj is IAudioCaptureClient audioCaptureClient) || audioCaptureClient == null)
            {
                throw new Exception(error);
            }

            return new ManagedAudioCapture(this, audioCaptureClient, sink, cancellationToken);
        }

        public ManagedAudioRender GetAudioRenderer(IWaveAudioContainer audio, CancellationToken cancellationToken)
        {
            var error = "Could not get the audio renderer client";
            audioClient.GetService(new Guid(ClsId.IAudioRendererClient), out object audioRenderClientObj).ThrowIfNotSucceded(error);
            if (!(audioRenderClientObj is IAudioRenderClient audioRenderClient) || audioRenderClient == null)
            {
                throw new Exception(error);
            }

            var clockError = "Could not get the audio clock";
            audioClient.GetService(new Guid(ClsId.IAudioClock), out object audioClockObj).ThrowIfNotSucceded(clockError);
            if (!(audioClockObj is IAudioClock audioClock) || audioClock == null)
            {
                throw new Exception(clockError);
            }

            return new ManagedAudioRender(this, new ManagedAudioClock(audioClock), audioRenderClient, audio, cancellationToken);
        }

        public uint GetBufferSize()
        {
            audioClient
                .GetBufferSize(out uint bufferFrameCount)
                .ThrowIfNotSucceded("Could not get the buffer size");
            return bufferFrameCount;
        }

        public uint GetCurrentPadding()
        {
            audioClient
                .GetCurrentPadding(out uint frameCount)
                .ThrowIfNotSucceded("Could not get the current padding");
            return frameCount;
        }

        public void Start()
        {
            audioClient.Start().ThrowIfNotSucceded("Could not start the audio client");
        }

        public void Stop()
        {
            audioClient.Stop().ThrowIfNotSucceded("Could not stop the audio client");
        }

        public bool TryInitialize([NotNull] ManagedWaveFormat audioFormat, out ManagedWaveFormat supportedFormat)
        {
            var isFormatSupportedResult = audioClient.IsFormatSupported(AudioClientShareMode.Shared, audioFormat.Ptr, out IntPtr avaiableFormatPtr);
            if (isFormatSupportedResult != HResult.S_OK)
            {
                supportedFormat = new ManagedWaveFormat(avaiableFormatPtr);
                return false;
            }

            // TODO: loopback recording support https://docs.microsoft.com/en-us/windows/win32/coreaudio/loopback-recording
            var initializeResult = audioClient.Initialize(AudioClientShareMode.Shared, AudioClientStreamFlags.None, ManagedAudioRender.RefimesPerSec, 0, audioFormat.Ptr, Guid.Empty);
            if (initializeResult != HResult.S_OK)
            {
                throw new Exception("Could not intitialize the audio client");
            }

            supportedFormat = audioFormat;
            InitializedFormat = audioFormat;
            return true;
        }

        public void Dispose()
        {
            Marshal.ReleaseComObject(audioClient);
        }
    }
}
