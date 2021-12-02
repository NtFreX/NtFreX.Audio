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
        private static readonly object LockObj = new object();
        private static SampleDefinition? definition;
        private static double ToValue(Sample sample)
        {
            // TODO: Make this tread safe and keep it allocation free.
            // The catch is we do not want to/cannot enumerate multiple times and the method should not allocate the whole array.
            // currently there is no need for thread safty but it is very likely that it will become nessesary to support.
            if (definition == null)
            {
                definition = sample.Definition;
            }
            Debug.Assert(definition == sample.Definition, "Only samples with the same definition can be averaged");

            return sample.AsNumber();
        }
        private static Func<Sample, double> ToValueFunc { get; } = ToValue;

        public static Sample Average(this IEnumerable<Sample> samples)
        {
            lock (LockObj)
            {
                var average = samples.Average(ToValueFunc);

                if (definition == null)
                {
                    throw new Exception("The enumeration cannot be empty");
                }

                return new Sample(average, definition!.Value);
            }
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
