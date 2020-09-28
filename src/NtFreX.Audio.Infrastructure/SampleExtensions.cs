using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NtFreX.Audio.Infrastructure
{
    public static class SampleExtensions
    {
        public static Sample Average(this IEnumerable<Sample> samples)
        {
            decimal sum = 0;
            var data = samples.ToArray();
            foreach (var sample in data)
            {
                if (data[0].Definition != sample.Definition)
                {
                    throw new Exception("The given samples are not of the same type");
                }
                sum += (decimal)sample.Value;
            }
            return new Sample((double)(sum / data.Length), data[0].Definition);
        }

        public static ISeekableAsyncEnumerable<Sample> ToSamplesAsync(this ISeekableAsyncEnumerable<byte[]> data, IAudioFormat format, bool isLittleEndian, CancellationToken cancellationToken = default)
        {
            _ = data ?? throw new ArgumentNullException(nameof(data));
            _ = format ?? throw new ArgumentNullException(nameof(format));

            return data.SelectManyAsync(
                x => ByteArrayToSamples(x, format, isLittleEndian), 
                data.GetDataLength(), 
                cancellationToken);
        }

        private static IEnumerable<Sample> ByteArrayToSamples(IReadOnlyList<byte> data, IAudioFormat format, bool isLittleEndian)
        {
            var value = data.ToArray();
            Debug.Assert(value.Length == format.BytesPerSample, "GetNextAudioAsync must return arrays the size of format.BytesPerSample");

            yield return new Sample(value.ToArray(), new SampleDefinition(format.Type, format.BitsPerSample, isLittleEndian));
        }
    }
}
