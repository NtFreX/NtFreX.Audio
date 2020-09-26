using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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

        [return: NotNull]
        public static async IAsyncEnumerable<Sample> ToAudioSamplesAsync(this IAsyncEnumerable<byte[]> audio, SampleDefinition definition, [MaybeNull][EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var sampleSize = definition.Bytes;
            await foreach (var buffer in audio.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                var currentIndex = 0;
                while (buffer.Length > currentIndex)
                {
                    yield return new Sample(buffer.AsMemory(currentIndex, sampleSize).ToArray(), definition);
                    currentIndex += sampleSize;
                }

                Debug.Assert(currentIndex == buffer.Length, $"Byte buffers can only be converted to audio samples when their size is a multiple of the sample size '{sampleSize}'");
            }
        }
    }
}
