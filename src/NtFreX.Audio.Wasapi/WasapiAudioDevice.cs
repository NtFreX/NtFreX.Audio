using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Wasapi.Interop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Wasapi
{
    internal enum WaveFormatType: ushort
    {
        WAVE_FORMAT_PCM = 0x0001,
        WAVE_FORMAT_EXTENSIBLE = 0xFFFE
    }


    internal struct WaveFormat
    {
        internal struct WaveFormatInner
        {
            public WaveFormatType FormatTag;
            public ushort Channels;
            public uint SamplesPerSec;
            public uint AvgBytesPerSec;
            public ushort BlockAlign;
            public ushort BitsPerSample;
            public ushort cbSize;
        }

        public WaveFormatInner Format;

        internal struct SamplesInner
        {
            public ushort wValidBitsPerSample;
            public ushort wSamplesPerBlock;
            public ushort wReserved;
        }

        public SamplesInner Samples;
        public uint dwChannelMask;
        public Guid SubFormat;
    };


    public class WasapiAudioDevice : IAudioDevice
    {
        [return: NotNull]
        public Task<IPlaybackContext> PlayAsync([NotNull] IWaveStreamAudioContainer audio, CancellationToken cancellationToken = default)
        {
            Guid deviceEnumeratorId = new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E");
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

            if(device.GetId(out var id) != HResult.S_OK || string.IsNullOrEmpty(id))
            {
                throw new Exception("Could not get the device id");
            }

            if(device.Activate(new Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2"), ClsCtx.LocalServer, IntPtr.Zero, out object audioClientObj) != HResult.S_OK || !(audioClientObj is IAudioClient audioClient) || audioClient == null)
            {
                throw new Exception("Could not activate device");
            }

            //audioClient.IsFormatSupported
            //https://github.com/jamesjharper/nFundamental/blob/29551b9457e4147ddeb5e944d3322a93a9178317/src/nFundamental.Wave/Format/WaveFormat.cs
            var bitsPerSample = 8;
            var channels = 1;
            var sampleRate = 44100;
            var format = new WaveFormat
            {
                Format = new WaveFormat.WaveFormatInner
                {
                    BitsPerSample = (ushort)bitsPerSample,
                    Channels = (ushort)channels,
                    BlockAlign = (ushort)(channels * bitsPerSample / 8),
                    SamplesPerSec = (ushort)sampleRate,
                    AvgBytesPerSec = (ushort)(sampleRate * (channels * bitsPerSample / 8)),

                    FormatTag = WaveFormatType.WAVE_FORMAT_EXTENSIBLE,
                    cbSize = 22
                },
                Samples = new WaveFormat.SamplesInner
                {
                    wReserved = 0,
                    wSamplesPerBlock = 0,
                    wValidBitsPerSample = 8
                },
                dwChannelMask = 0x4
            };
            var formatPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(format));
            Marshal.StructureToPtr(format, formatPtr, false);

            if (audioClient.GetMixFormat(out IntPtr defaultFormatPtr) != HResult.S_OK)
            {
                throw new Exception("Could not get the mix format");
            }
            var defaultFormatToCompare = Marshal.PtrToStructure<WaveFormat>(defaultFormatPtr);

            const int REFTIMES_PER_SEC = 10000000;
            //var nFrames = 0;
            //var referenceTime = (ulong) ((10000.0 * 1000 / format.SamplesPerSec * nFrames) + 0.5);
            var initializeResult = audioClient.Initialize(AudioClientShareMode.Shared, AudioClientStreamFlags.None, REFTIMES_PER_SEC, 0, formatPtr, Guid.Empty);
            if (initializeResult != HResult.S_OK)
            {
                throw new Exception("Could not intitialize the audio client");
            }

            var getServiceResult = audioClient.GetService(new Guid("F294ACFC-3146-4483-A7BF-ADDCA7C260E2"), out object audioRenderClientObj);
            if (getServiceResult != HResult.S_OK || !(audioRenderClientObj is IAudioRenderClient audioRenderClient) || audioRenderClient == null)
            {
                throw new Exception("Could not get the audio renderer client", new COMException(null, (int) getServiceResult));
            }

            if(audioRenderClient.GetBuffer(20, out IntPtr dataBufferPointer) != HResult.S_OK || dataBufferPointer == IntPtr.Zero)
            {
                throw new Exception("Could not get the data buffer");
            }
            audioClient.Start();

            // fill buffer continuesly

            audioClient.Stop();

            throw new Exception();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
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
    [Guid("F294ACFC-3146-4483-A7BF-ADDCA7C260E2")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioRenderClient
    {
        [PreserveSig]
        HResult GetBuffer(int numFramesRequested, out IntPtr dataBufferPointer);

        [PreserveSig]
        HResult ReleaseBuffer(int numFramesWritten, AudioClientBufferFlags bufferFlags);
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
    [Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2")]
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
