using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;

namespace NtFreX.Audio.Samplers
{
    public static class WaveStretcher
    {
        public static ISeekableAsyncEnumerable<Sample> StretchAsync(IntermediateEnumerableAudioContainer audio, double factor, CancellationToken cancellationToken)
            => StretchInnerAsync(audio, factor, cancellationToken).ToSeekable(audio, (long)(audio.GetDataLength() * factor));

        private static async IAsyncEnumerable<Sample> StretchInnerAsync(IntermediateEnumerableAudioContainer audio, double factor, CancellationToken cancellationToken)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if(factor > 2 || factor < 0.5)
            {
                throw new ArgumentException("Factor out of range", nameof(factor));
            }

            var size = audio.GetByteLength();
            var format = audio.GetFormat();
            var newDataSize = System.Math.Round(factor * size, 0);
            var sizeOfParts = size / (double)System.Math.Abs(size - newDataSize);
            var previous = Sample.Zero(new SampleDefinition(format.Type, format.BitsPerSample, audio.IsDataLittleEndian()));
            var counter = 1d;
            var total = 0L;
            await foreach(var sample in audio)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                var positionReached = counter > sizeOfParts;

                // upsampling
                if (factor > 1)
                {
                    if (positionReached)
                    {
                        yield return new Sample[] { sample, previous }.Average();
                        counter -= sizeOfParts;
                    }

                    yield return sample;
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
                        yield return sample;
                        total += format.BytesPerSample;
                    }
                }

                counter++;
                previous = sample;
            }

            if (factor > 1 && counter > sizeOfParts && factor <= 2)
            {
                yield return previous / 2;
            }
        }
    }
}
