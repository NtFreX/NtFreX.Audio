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

        public ManagedAudioRender GetAudioRenderer(IWaveAudioContainer audio, CancellationToken cancellationToken)
        {
            var error = "Could not get the audio renderer client";
            audioClient.GetService(new Guid(ClsId.IAudioRendererClient), out object audioRenderClientObj).ThrowIfNotSucceded(error);
            if (!(audioRenderClientObj is IAudioRenderClient audioRenderClient) || audioRenderClient == null)
            {
                throw new Exception(error);
            }
            return new ManagedAudioRender(this, audioRenderClient, audio, cancellationToken);
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

            var initializeResult = audioClient.Initialize(AudioClientShareMode.Shared, AudioClientStreamFlags.None, ManagedAudioRender.REFTIMES_PER_SEC, 0, audioFormat.Ptr, Guid.Empty);
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
