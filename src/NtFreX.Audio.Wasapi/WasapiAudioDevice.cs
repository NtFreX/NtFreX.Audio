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

    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/mmreg/ns-mmreg-waveformatextensible
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    internal struct WaveFormatDefinition
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        internal struct FormatDefinition
        {
            public WaveFormatType FormatTag;
            public ushort Channels;
            public uint SamplesPerSec;
            public uint AvgBytesPerSec;
            public ushort BlockAlign;
            public ushort BitsPerSample;
            public ushort Size;
        }

        public FormatDefinition Format;

        [StructLayout(LayoutKind.Explicit)]
        internal struct SamplesDefintion
        {
            [FieldOffset(0)]
            public ushort ValidBitsPerSample;

            [FieldOffset(0)]
            public ushort SamplesPerBlock;

            [FieldOffset(0)]
            public ushort Reserved;
        }

        public SamplesDefintion Samples;
        public Speaker ChannelMask;
        public Guid SubFormat;
    }

    internal static class MediaType
    {
        // Ksmedia.h 
        public static Guid STATIC_KSDATAFORMAT_SUBTYPE_PCM => new Guid("00000001-0000-0010-8000-00aa00389b71");
    }

    public class WasapiAudioDevice : IAudioDevice
    {
        private  WaveFormatDefinition CreateInstance(ushort channels, uint sampleRate, ushort bitsPerSample, Speaker speaker, Guid format)
        {
            return new WaveFormatDefinition {
                Format = new WaveFormatDefinition.FormatDefinition
                {
                    BitsPerSample = bitsPerSample,
                    Channels = channels,
                    BlockAlign = (ushort)(channels * bitsPerSample / 8),
                    SamplesPerSec = sampleRate,
                    AvgBytesPerSec = (uint)(sampleRate * (channels * bitsPerSample / 8)),

                    FormatTag = WaveFormatType.EXTENSIBLE,
                    Size = 22
                },
                Samples = new WaveFormatDefinition.SamplesDefintion
                {
                    Reserved = bitsPerSample,
                    SamplesPerBlock = bitsPerSample,
                    ValidBitsPerSample = bitsPerSample
                },
                ChannelMask = speaker,
                SubFormat = format
            };
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/coreaudio/rendering-a-stream
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [return: NotNull]
        public Task<IPlaybackContext> PlayAsync([NotNull] IWaveAudioContainer audio, CancellationToken cancellationToken = default)
        {
            var device = MultiMediaDeviceEnumerator.Instance.GetDefaultRenderDevice();

            //if(device.GetId(out var id) != HResult.S_OK || string.IsNullOrEmpty(id))
            //{
            //    throw new Exception("Could not get the device id");
            //}

            if(device.Activate(new Guid(ClsId.IAudioClient), ClsCtx.LocalServer, IntPtr.Zero, out object audioClientObj) != HResult.S_OK || !(audioClientObj is IAudioClient audioClient) || audioClient == null)
            {
                throw new Exception("Could not activate device");
            }

            //if (audioClient.GetMixFormat(out IntPtr formatPtr) != HResult.S_OK)
            //{
            //    throw new Exception("Could not get the mix format");
            //}
            //var format = Marshal.PtrToStructure<WaveFormatDefinition>(formatPtr);
            //format.Format.BitsPerSample = audio.FmtSubChunk.BitsPerSample;
            //format.Format.SamplesPerSec = audio.FmtSubChunk.SampleRate;
            //format.Format.Channels = audio.FmtSubChunk.NumChannels;
            //format.Format.BlockAlign = (ushort)(audio.FmtSubChunk.NumChannels * audio.FmtSubChunk.BitsPerSample / 8);
            //format.Format.AvgBytesPerSec = (uint)(audio.FmtSubChunk.SampleRate * (audio.FmtSubChunk.NumChannels * audio.FmtSubChunk.BitsPerSample / 8));
            //Marshal.StructureToPtr(format, formatPtr, true);

            ////https://github.com/jamesjharper/nFundamental/blob/29551b9457e4147ddeb5e944d3322a93a9178317/src/nFundamental.Wave/Format/WaveFormat.cs
            ////TODO: correct format
            ushort bitsPerSample = audio.FmtSubChunk.BitsPerSample;
            ushort channels = audio.FmtSubChunk.NumChannels;
            uint sampleRate = audio.FmtSubChunk.SampleRate;
            var format = CreateInstance(channels, sampleRate, bitsPerSample, Speaker.FRONT_LEFT | Speaker.FRONT_RIGHT, MediaType.STATIC_KSDATAFORMAT_SUBTYPE_PCM);
            //var format = CreateInstance(channels, 48000, bitsPerSample, Speaker.FRONT_LEFT | Speaker.FRONT_RIGHT, MediaType.STATIC_KSDATAFORMAT_SUBTYPE_PCM);
            var formatPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(format));
            Marshal.StructureToPtr(format, formatPtr, false);

            var isFormatSupportedResult = audioClient.IsFormatSupported(AudioClientShareMode.Shared, formatPtr, out IntPtr avaiableFormat);
            if (isFormatSupportedResult != HResult.S_OK)
            {
                throw new Exception("The given format is not supported");
            }

            var initializeResult = audioClient.Initialize(AudioClientShareMode.Shared, AudioClientStreamFlags.None, WasapiPlaybackContext.REFTIMES_PER_SEC, 0, formatPtr, Guid.Empty);
            if (initializeResult != HResult.S_OK)
            {
                throw new Exception("Could not intitialize the audio client");
            }

            // todo: dispose/cleanup
            var context = new WasapiPlaybackContext(audio, audioClient, format, cancellationToken);
            return Task.FromResult(context as IPlaybackContext);
        }


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

        internal WasapiPlaybackContext(IWaveAudioContainer audio, IAudioClient audioClient, WaveFormatDefinition format, CancellationToken cancellationToken)
        {
            var getServiceResult = audioClient.GetService(new Guid(ClsId.IAudioRendererClient), out object audioRenderClientObj);
            if (getServiceResult != HResult.S_OK || !(audioRenderClientObj is IAudioRenderClient audioRenderClient) || audioRenderClient == null)
            {
                throw new Exception("Could not get the audio renderer client", new COMException(null, (int)getServiceResult));
            }

            if (audioClient.GetBufferSize(out uint bufferFrameCount) != HResult.S_OK)
            {
                throw new Exception("Could not get the buffer size");
            }

            this.audio = audio;
            this.audioClient = audioClient;
            this.bufferFrameCount = bufferFrameCount;
            this.audioRenderClient = audioRenderClient;
            this.format = format;
            this.cancellationToken = cancellationToken;

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
