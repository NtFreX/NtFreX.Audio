using Dasync.Collections;
using NtFreX.Audio.Infrastructure;
using System;
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
    }
}
