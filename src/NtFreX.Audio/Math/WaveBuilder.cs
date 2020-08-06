using Dasync.Collections;
using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NtFreX.Audio.Math
{
    public static class WaveBuilder
    {
        public static byte[] Silence(IAudioFormat format, int lengthInSeconds, bool isLittleEndian = true)
        {
            _ = format ?? throw new ArgumentNullException(nameof(format));

            return Enumerable
                .Repeat(0L, (int)(format.SampleRate * format.Channels * lengthInSeconds))
                .Select(x => x.ToByteArray(format.BitsPerSample / 8, isLittleEndian))
                .SelectMany(x => x)
                .ToArray();
        }

        public static byte[] Sin(IAudioFormat format, int waveWidth, int lengthInSeconds)
        {
            _ = format ?? throw new ArgumentNullException(nameof(format));

            if (format.Type != AudioFormatType.IeeFloat || format.BitsPerSample != 64)
            {
                throw new ArgumentException("Only iee float with 64 bits per sample is supported", nameof(format));
            }

            return Sin(waveWidth, format.SampleRate, format.Channels, lengthInSeconds)
                .Select(x => BitConverter.GetBytes(x))
                .SelectMany(x => x)
                .ToArray();
        }

        private static IEnumerable<double> Sin(int waveWidth, uint sampleRate, uint channels, int lengthInSeconds)
        {
            var wave = Sin(waveWidth).ToArray();
            var length = sampleRate * channels * lengthInSeconds;
            var waveIndex = 0;
            for (var i = 0; i < length; i++)
            {
                if (++waveIndex >= wave.Length)
                {
                    waveIndex = 0;
                }

                yield return wave[waveIndex];
            }
        }

        private static IEnumerable<double> Sin(int waveWidth)
        {
            for (double i = -2; i <= 2; i += 4d / waveWidth * 2)
            {
                yield return System.Math.Sin(System.Math.PI * i);
            }
        }
    }
}
