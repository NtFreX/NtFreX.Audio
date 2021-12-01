using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NtFreX.Audio.Math
{
    public static class WaveBuilder
    {
        // TODO: provide seekable async enumerable in all methods in this type
        // TODO: pass cancelation token
        public static ISeekableAsyncEnumerable<byte> Silence(IAudioFormat format, int lengthInSeconds, bool isLittleEndian = true)
        {
            _ = format ?? throw new ArgumentNullException(nameof(format));

            var valueToRepeat = NumberFactory.DeconstructNumber(new SampleDefinition(format.Type, format.BitsPerSample, isLittleEndian), 0d);

            return Enumerable
                .Repeat(valueToRepeat, (int)(format.SampleRate * format.Channels * lengthInSeconds))
                .SelectMany(x => x.ToArray())
                .ToAsyncEnumerable()
                .ToNonSeekable((ulong)(lengthInSeconds * format.SampleRate * format.BytesPerSample * format.Channels));
        }

        /// <summary>
        /// Generates a sin wave
        /// </summary>
        /// <param name="sampleRate">The sample rate of the audio</param>
        /// <param name="frequency">The frequency of the wave in hz</param>
        /// <param name="lengthInSeconds">The length of the audio</param>
        /// <returns>64 bit iee float mono audio</returns>
        public static ISeekableAsyncEnumerable<byte> Sin(uint sampleRate, int frequency, int lengthInSeconds)
        {
            return SinIeeFloat64(sampleRate, frequency, lengthInSeconds)
                .Select(x => BitConverter.GetBytes(x))
                .SelectMany(x => x)
                .ToAsyncEnumerable()
                .ToNonSeekable((ulong)(8 /* bytesPerSample */ * 1 /* channels */ * sampleRate * lengthInSeconds));
        }

        private static IEnumerable<double> SinIeeFloat64(uint sampleRate, int frequency, int lengthInSeconds)
        {
            for (var sampleNumber = 0; sampleNumber < sampleRate * lengthInSeconds; sampleNumber++)
            {
                var timeInSeconds = sampleNumber / (sampleRate * 1f);
                yield return System.Math.Sin(2 * System.Math.PI * frequency * timeInSeconds);
            }
        }
    }
}
