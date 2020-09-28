using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NtFreX.Audio.Samplers
{
    public static class WaveStretcher
    {
        public static ISeekableAsyncEnumerable<Sample> StretchAsync(IntermediateEnumerableAudioContainer audio, double factor, CancellationToken cancellationToken)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var enumerator = audio.GetAsyncEnumerator(cancellationToken);
            var enumerable = StretchInnerAsync(enumerator, audio.GetFormat(), audio.IsDataLittleEndian(), audio.GetByteLength(), factor, cancellationToken);
            return enumerable.ToSeekable(enumerator, audio.DisposeAsync, (long)(audio.GetDataLength() * factor));
        }

        private static async IAsyncEnumerable<Sample> StretchInnerAsync(ISeekableAsyncEnumerator<Sample> audio, IAudioFormat format, bool isLittleEndian, long sizeInBytes, double factor, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if(factor > 2 || factor < 0.5)
            {
                throw new ArgumentException("Factor out of range", nameof(factor));
            }

            var newDataSize = System.Math.Round(factor * sizeInBytes, 0);
            var sizeOfParts = sizeInBytes / (double)System.Math.Abs(sizeInBytes - newDataSize);
            var previous = Sample.Zero(new SampleDefinition(format.Type, format.BitsPerSample, isLittleEndian));
            var counter = 1d;
            var total = 0L;
            while(await audio.MoveNextAsync().ConfigureAwait(false))
            {
                var sample = audio.Current;
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
