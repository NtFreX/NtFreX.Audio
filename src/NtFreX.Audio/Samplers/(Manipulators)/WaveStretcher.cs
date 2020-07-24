using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public static class WaveStretcher
    {
        [return: NotNull]
        public static async IAsyncEnumerable<Sample> StretchAsync([NotNull] WaveEnumerableAudioContainer audio, double factor, [MaybeNull][EnumeratorCancellation] CancellationToken cancellationToken)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if(factor > 2 || factor < 0.5)
            {
                throw new ArgumentException("Factor out of range", nameof(factor));
            }

            var newDataSize = System.Math.Round(factor * audio.DataSubChunk.ChunkSize, 0);
            var sizeOfParts = audio.DataSubChunk.ChunkSize / (double)System.Math.Abs(audio.DataSubChunk.ChunkSize - newDataSize);
            var previous = Sample.Zero(audio.FmtSubChunk.BitsPerSample, audio.FmtSubChunk.AudioFormat, audio.IsDataLittleEndian());
            var counter = 1d;
            var total = 0L;
            await foreach (var value in audio.GetAudioSamplesAsync(cancellationToken).ConfigureAwait(false))
            {
                var positionReached = counter > sizeOfParts;

                // upsampling
                if (factor > 1)
                {
                    if (positionReached)
                    {
                        yield return new Sample[] { value, previous }.Average();
                        counter -= sizeOfParts;
                    }

                    yield return value;
                }

                // downsampling
                if (factor < 1)
                {
                    if (positionReached)
                    {
                        counter -= sizeOfParts;
                    }
                    else if (total < newDataSize)
                    {
                        yield return value;
                        total += audio.FmtSubChunk.BitsPerSample / 8;
                    }
                }

                counter++;
                previous = value;
            }

            if (factor > 1 && counter > sizeOfParts && factor <= 2)
            {
                yield return previous / 2;
            }
        }
    }
}
