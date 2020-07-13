using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Wasapi.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Wasapi
{
    [Flags]
    internal enum Speaker: uint
    {
        FRONT_LEFT = 0x1,
        FRONT_RIGHT	= 0x2,
        FRONT_CENTER = 0x4,
        LOW_FREQUENCY = 0x8,
        BACK_LEFT = 0x10,
        BACK_RIGHT = 0x20,
        FRONT_LEFT_OF_CENTER = 0x40,
        FRONT_RIGHT_OF_CENTER = 0x80,
        BACK_CENTER = 0x100,
        SIDE_LEFT = 0x200,
        SIDE_RIGHT = 0x400,
        TOP_CENTER = 0x800,
        TOP_FRONT_LEFT = 0x1000,
        TOP_FRONT_CENTER = 0x2000,
        TOP_FRONT_RIGHT = 0x4000,
        TOP_BACK_LEFT = 0x8000,
        TOP_BACK_CENTER = 0x10000,
        TOP_BACK_RIGHT = 0x20000
    }

    internal enum WaveFormatType: ushort
    {
        PCM = 0x0001,
        EXTENSIBLE = 0xFFFE
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/api/mmreg/ns-mmreg-waveformatextensible
    /// </summary>
    internal struct WaveFormatDefinition
    {
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

        internal struct SamplesDefintion
        {
            public ushort ValidBitsPerSample;
            public ushort SamplesPerBlock;
            public ushort Reserved;
        }

        public SamplesDefintion Samples;
        public Speaker ChannelMask;
        public Guid SubFormat;

        public WaveFormatDefinition(ushort channels, uint sampleRate, ushort bitsPerSample)
        {
            Format = new FormatDefinition
            {
                BitsPerSample = bitsPerSample,
                Channels = channels,
                BlockAlign = (ushort)(channels * bitsPerSample / 8),
                SamplesPerSec = sampleRate,
                AvgBytesPerSec = (uint)(sampleRate * (channels * bitsPerSample / 8)),

                FormatTag = WaveFormatType.EXTENSIBLE,
                Size = 22
            };
            Samples = new SamplesDefintion
            {
                Reserved = 0,
                SamplesPerBlock = 0,
                ValidBitsPerSample = 8
            };
            ChannelMask = Speaker.FRONT_CENTER;
            SubFormat = new Guid("00000001-0000-0010-8000-00aa00389b71"); // Ksmedia.h STATIC_KSDATAFORMAT_SUBTYPE_PCM
        }
    }

    internal static class ClsId
    {
        public const string MMDeviceEnumerator = "BCDE0395-E52F-467C-8E3D-C4579291692E";
        public const string IMMDeviceEnumerator = "A95664D2-9614-4F35-A746-DE8DB63617E6";

        public const string IAudioClient = "1CB9AD4C-DBFA-4c32-B178-C2F568A703B2";
        public const string IAudioRendererClient = "F294ACFC-3146-4483-A7BF-ADDCA7C260E2";
    }

    public class WasapiAudioDevice : IAudioDevice
    {
        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/coreaudio/rendering-a-stream
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [return: NotNull]
        public async Task<IPlaybackContext> PlayAsync([NotNull] IWaveStreamAudioContainer audio, CancellationToken cancellationToken = default)
        {
            Guid deviceEnumeratorId = new Guid(ClsId.MMDeviceEnumerator);
            Type deviceEnumeratorType = Type.GetTypeFromCLSID(deviceEnumeratorId, true);
            var  deviceEnumerator = Activator.CreateInstance(deviceEnumeratorType) as IMMDeviceEnumerator;
            if(deviceEnumerator == null)
            {
                throw new Exception("Could not get the device enumerator");
            }

            if(deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console, out IMMDevice device) != HResult.S_OK || device == null)
            {
                throw new Exception("Could not get the default renderer device");
            }

            //if(device.GetId(out var id) != HResult.S_OK || string.IsNullOrEmpty(id))
            //{
            //    throw new Exception("Could not get the device id");
            //}

            if(device.Activate(new Guid(ClsId.IAudioClient), ClsCtx.LocalServer, IntPtr.Zero, out object audioClientObj) != HResult.S_OK || !(audioClientObj is IAudioClient audioClient) || audioClient == null)
            {
                throw new Exception("Could not activate device");
            }

            if (audioClient.GetMixFormat(out IntPtr formatPtr) != HResult.S_OK)
            {
                throw new Exception("Could not get the mix format");
            }
            var format = Marshal.PtrToStructure<WaveFormatDefinition>(formatPtr);

            ////https://github.com/jamesjharper/nFundamental/blob/29551b9457e4147ddeb5e944d3322a93a9178317/src/nFundamental.Wave/Format/WaveFormat.cs
            ////TODO: correct format
            //ushort bitsPerSample = 16;
            //ushort channels = 1;
            //uint sampleRate = 44100;
            //var format = new WaveFormatDefinition(channels, sampleRate, bitsPerSample);
            //var formatPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(format));
            //Marshal.StructureToPtr(format, formatPtr, false);

            //var isFormatSupportedResult = audioClient.IsFormatSupported(AudioClientShareMode.Shared, formatPtr, out IntPtr avaiableFormat);
            //if (isFormatSupportedResult != HResult.S_OK)
            //{
            //    throw new Exception("The given format is not supported");
            //}

            const int REFTIMES_PER_SEC = 10000000;
            const int REFTIMES_PER_MILLISEC = 10000;
            var initializeResult = audioClient.Initialize(AudioClientShareMode.Shared, AudioClientStreamFlags.None, REFTIMES_PER_SEC, 0, formatPtr, Guid.Empty);
            if (initializeResult != HResult.S_OK)
            {
                throw new Exception("Could not intitialize the audio client");
            }

            var getServiceResult = audioClient.GetService(new Guid(ClsId.IAudioRendererClient), out object audioRenderClientObj);
            if (getServiceResult != HResult.S_OK || !(audioRenderClientObj is IAudioRenderClient audioRenderClient) || audioRenderClient == null)
            {
                throw new Exception("Could not get the audio renderer client", new COMException(null, (int) getServiceResult));
            }

            if(audioClient.GetBufferSize(out uint bufferSize) != HResult.S_OK)
            {
                throw new Exception("Could not get the buffer size");
            }

            if (audioClient.Start() != HResult.S_OK)
            {
                throw new Exception("Could not start the audio client");
            }

            var realBuffer = new List<byte>();
            var hnsActualDuration = (double)REFTIMES_PER_SEC *
                        bufferSize / format.Format.SamplesPerSec;
            await foreach(var buffer in audio.GetAudioSamplesAsync(cancellationToken).ConfigureAwait(false).WithCancellation(cancellationToken))
            {
                realBuffer.AddRange(buffer);

                if(realBuffer.Count >= bufferSize)
                {
                    await Task.Delay((int)(hnsActualDuration / REFTIMES_PER_MILLISEC / 2)).ConfigureAwait(false);

                    // See how much buffer space is available.
                    if(audioClient.GetCurrentPadding(out uint numFramesPadding) != HResult.S_OK)
                    {
                        throw new Exception("Could not get the current padding");
                    }

                    var numFramesAvailable = bufferSize - numFramesPadding;

                    if (audioRenderClient.GetBuffer(numFramesAvailable, out IntPtr dataBufferPointer) != HResult.S_OK || dataBufferPointer == IntPtr.Zero)
                    {
                        throw new Exception("Could not get the data buffer");
                    }

                    Marshal.Copy(realBuffer.ToArray(), 0, dataBufferPointer, (int)numFramesAvailable);
                    realBuffer.RemoveRange(0, (int)numFramesAvailable);

                    if (audioRenderClient.ReleaseBuffer(numFramesAvailable, AudioClientBufferFlags.None) != HResult.S_OK)
                    {
                        throw new Exception("Could not release the buffer");
                    }
                }                
            }

            // todo: dispose/cleanup
            if(audioClient.Stop() != HResult.S_OK)
            {
                throw new Exception("Could not stop the audio client");
            }

            return null;
        }

        public void Dispose()
        {
            //TODO: implement
        }
    }


    /// <summary>
    /// Audio Client Buffer Flags
    /// </summary>
    [Flags]
    internal enum AudioClientBufferFlags
    {
        None,
        DataDiscontinuity = 0x1,
        Silent = 0x2,
        TimestampError = 0x4

    }

    [ComImport]
    [Guid(ClsId.IAudioRendererClient)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioRenderClient
    {
        [PreserveSig]
        HResult GetBuffer(uint numFramesRequested, out IntPtr dataBufferPointer);

        [PreserveSig]
        HResult ReleaseBuffer(uint numFramesWritten, AudioClientBufferFlags bufferFlags);
    }

    [Flags]
    internal enum AudioClientStreamFlags
    {
        None,
        CrossProcess = 0x00010000,
        Loopback = 0x00020000,
        EventCallback = 0x00040000,
        NoPersist = 0x00080000,
    }

    internal enum AudioClientShareMode
    {
        Shared = 0,
        Exclusive = 1,
    }


    [ComImport]
    [Guid(ClsId.IAudioClient)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioClient
    {

        [PreserveSig]
        HResult Initialize([In][MarshalAs(UnmanagedType.I4)] AudioClientShareMode shareMode,
                            [In][MarshalAs(UnmanagedType.U4)] AudioClientStreamFlags streamFlags,
                            [In][MarshalAs(UnmanagedType.U8)] UInt64 bufferDuration,
                            [In][MarshalAs(UnmanagedType.U8)] UInt64 devicePeriod,
                            [In][MarshalAs(UnmanagedType.SysInt)] IntPtr format,
                            [In, Optional][MarshalAs(UnmanagedType.LPStruct)] Guid audioSessionId);

        [PreserveSig]
        HResult GetBufferSize([Out][MarshalAs(UnmanagedType.U4)] out UInt32 size);

        [PreserveSig]
        HResult GetStreamLatency([Out][MarshalAs(UnmanagedType.U8)] out UInt64 latency);

        [PreserveSig]
        HResult GetCurrentPadding([Out][MarshalAs(UnmanagedType.U4)] out UInt32 frameCount);

        [PreserveSig]
        HResult IsFormatSupported([In][MarshalAs(UnmanagedType.I4)] AudioClientShareMode shareMode,
                                   [In][MarshalAs(UnmanagedType.SysInt)] IntPtr format,
                                   [Out] out IntPtr closestMatch);

        [PreserveSig]
        HResult GetMixFormat([Out][MarshalAs(UnmanagedType.SysInt)] out IntPtr format);

        [PreserveSig]
        HResult GetDevicePeriod([Out][MarshalAs(UnmanagedType.U8)] out UInt64 defaultDevicePeriod,
                                [Out][MarshalAs(UnmanagedType.U8)] out UInt64 minimumDevicePeriod);

        [PreserveSig]
        HResult Start();

        [PreserveSig]
        HResult Stop();

        [PreserveSig]
        HResult Reset();

        [PreserveSig]
        HResult SetEventHandle([In][MarshalAs(UnmanagedType.SysInt)] IntPtr handle);

        [PreserveSig]
        HResult GetService([In][MarshalAs(UnmanagedType.LPStruct)] Guid interfaceId,
                           [Out][MarshalAs(UnmanagedType.IUnknown)] out object instancePtr);
    }
}
