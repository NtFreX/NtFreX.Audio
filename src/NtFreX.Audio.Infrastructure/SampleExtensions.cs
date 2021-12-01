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
            var data = samples.ToArray();
            var average = data.Average(sample => sample.AsNumber());
            return new Sample(average, data[0].Definition);
        }

        public static ISeekableAsyncEnumerable<Sample> ToSamplesAsync(this ISeekableAsyncEnumerable<Memory<byte>> data, ulong? realByteLength, IAudioFormat format, bool isLittleEndian, CancellationToken cancellationToken = default)
        {
            _ = data ?? throw new ArgumentNullException(nameof(data));
            _ = format ?? throw new ArgumentNullException(nameof(format));

            return data.SelectManyAsync(
                x => BytesToSamples(x, format, isLittleEndian),
                realByteLength == null ? null : realByteLength / format.BytesPerSample, 
                cancellationToken);
        }

        private static IEnumerable<Sample> BytesToSamples(Memory<byte> data, IAudioFormat format, bool isLittleEndian)
        {
            Debug.Assert(data.Length % format.BytesPerSample == 0, "GetNextAudioAsync must return arrays which size are multiplications of format.BytesPerSample");

            for (var i = 0; i < data.Length; i += format.BytesPerSample)
            {
                yield return new Sample(data.Slice(i, format.BytesPerSample), new SampleDefinition(format.Type, format.BitsPerSample, isLittleEndian));
            }
        }
    }
}
