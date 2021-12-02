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
        public static Sample Average(this Memory<Sample> samples)
        {
            double sum = 0;
            long count = 0;
            SampleDefinition? definition = null;
            checked
            {
                for (var index = 0; index < samples.Length; index++)
                {
                    var item = samples.Span[index];
                    sum += item.AsNumber();
                    count++;

                    if (definition == null)
                    {
                        definition = item.Definition;
                    }
                    Debug.Assert(definition == item.Definition, "Only samples with the same definition can be averaged");
                }
            }

            if (definition == null)
            {
                throw new Exception("The enumeration cannot be empty");
            }

            var average = (double)sum / count;
            var sample = new Sample(average, definition!.Value);
            return sample;
        }

        public static Sample Average(this IEnumerable<Sample> samples)
        {
            _ = samples ?? throw new ArgumentNullException(nameof(samples));

            double sum = 0;
            long count = 0;
            SampleDefinition? definition = null;
            checked
            {
                foreach (var item in samples)
                {
                    sum += item.AsNumber();
                    count++;

                    if (definition == null)
                    {
                        definition = item.Definition;
                    }
                    Debug.Assert(definition == item.Definition, "Only samples with the same definition can be averaged");
                }
            }

            if (definition == null)
            {
                throw new Exception("The enumeration cannot be empty");
            }

            var average = (double)sum / count;
            var sample = new Sample(average, definition!.Value);
            return sample;
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
