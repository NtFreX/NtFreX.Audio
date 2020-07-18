using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Wasapi.Interop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Wrapper
{
    internal class MultiMediaDevice
    {
        private readonly IMMDevice device;
        private IAudioClient audioClient;

        public MultiMediaDevice(IMMDevice device)
        {
            this.device = device;
        }

        public string GetId()
        {
            if (device.GetId(out var id) != HResult.S_OK || string.IsNullOrEmpty(id))
            {
                throw new Exception("Could not get the device id");
            }

            return id;
        }

        public AudioClient? TryGetAudioClient([NotNull] IWaveAudioContainer audio, out WaveFormatDefinition supportedFormat)
        {
            if (audioClient == null)
            {
                if (device.Activate(new Guid(ClsId.IAudioClient), ClsCtx.LocalServer, IntPtr.Zero, out object audioClientObj) != HResult.S_OK || !(audioClientObj is IAudioClient audioClient) || audioClient == null)
                {
                    throw new Exception("Could not activate device");
                }
                this.audioClient = audioClient;
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
            var format = new WaveFormatDefinition(channels, sampleRate, bitsPerSample, Speaker.FRONT_LEFT | Speaker.FRONT_RIGHT, MediaType.STATIC_KSDATAFORMAT_SUBTYPE_PCM);
            //var format = CreateInstance(channels, 48000, bitsPerSample, Speaker.FRONT_LEFT | Speaker.FRONT_RIGHT, MediaType.STATIC_KSDATAFORMAT_SUBTYPE_PCM);
            var formatPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(format));
            Marshal.StructureToPtr(format, formatPtr, false);

            var isFormatSupportedResult = audioClient.IsFormatSupported(AudioClientShareMode.Shared, formatPtr, out IntPtr avaiableFormatPtr);
            if (isFormatSupportedResult != HResult.S_OK)
            {
                supportedFormat = Marshal.PtrToStructure<WaveFormatDefinition>(avaiableFormatPtr);
                return null;
            }

            var initializeResult = audioClient.Initialize(AudioClientShareMode.Shared, AudioClientStreamFlags.None, WasapiPlaybackContext.REFTIMES_PER_SEC, 0, formatPtr, Guid.Empty);
            if (initializeResult != HResult.S_OK)
            {
                throw new Exception("Could not intitialize the audio client");
            }

            supportedFormat = format;
            return new AudioClient(audioClient, format);
        } 
    }

    internal class AudioClient
    {
        internal readonly IAudioClient audioClient;
        internal readonly WaveFormatDefinition format;

        public AudioClient(IAudioClient audioClient, WaveFormatDefinition format)
        {
            this.audioClient = audioClient;
            this.format = format;
        }
    }
}
