﻿using NtFreX.Audio.Infrastructure;
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

        /// <summary>
        /// Generates a sin wave
        /// </summary>
        /// <param name="sampleRate">The sample rate of the audio</param>
        /// <param name="frequency">The frequency of the wave in hz</param>
        /// <param name="lengthInSeconds">The length of the audio</param>
        /// <returns>64 bit iee float mono audio</returns>
        public static byte[] Sin(uint sampleRate, int frequency, int lengthInSeconds)
        {
            return SinIeeFloat64(sampleRate, frequency, lengthInSeconds)
                .Select(x => BitConverter.GetBytes(x))
                .SelectMany(x => x)
                .ToArray();
        }

        private static IEnumerable<double> SinIeeFloat64(uint sampleRate, int frequency, int lengthInSeconds)
        {
            for (var sampleNumber = 0; sampleNumber < sampleRate * lengthInSeconds; sampleNumber++)
            {
                var timeInSeconds = sampleNumber / (sampleRate * 1f);
                yield return System.Math.Sin(2 * System.Math.PI * frequency * timeInSeconds) + 1f;
            }
        }
    }
}