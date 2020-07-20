using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Interop
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

        public WaveFormatDefinition(ushort channels, uint sampleRate, ushort bitsPerSample, Speaker speaker, Guid format)
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
                ValidBitsPerSample = bitsPerSample
            };
            ChannelMask = speaker;
            SubFormat = format;
        }
    }
}
