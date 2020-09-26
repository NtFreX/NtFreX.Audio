using NtFreX.Audio.Containers;
using NtFreX.Audio.Containers.Wave;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Converters
{
    internal class WaveToIntermediateConverter : AudioConverter<WaveAudioContainer, IntermediateEnumerableAudioContainer>
    {
        protected override Task<IntermediateEnumerableAudioContainer> ConvertAsync(WaveAudioContainer from, CancellationToken cancellationToken = default)
        {
            var format = from.GetFormat();
            var isLittleEndian = from.IsDataLittleEndian();
            var container = new IntermediateEnumerableAudioContainer(
                ToSampleEnumerable(
                    format,
                    isLittleEndian,
                    from.GetAsyncAudioEnumerator(cancellationToken))
                .ToNonSeekable(from.DataSubChunk.ChunkSize / format.BytesPerSample),
                format,
                isLittleEndian);
            return Task.FromResult(container);
        }

        private static async IAsyncEnumerable<Sample> ToSampleEnumerable(IAudioFormat format, bool isLittleEndian, IAsyncEnumerator<IReadOnlyList<byte>> data)
        {
            while (await data.MoveNextAsync().ConfigureAwait(false))
            {
                var value = data.Current?.ToArray() ?? Array.Empty<byte>();
                Debug.Assert(value.Length % format.BytesPerSample == 0, "GetNextAudioAsync must return arrays the size of format.BitsPerSample / 8");

                for (var i = 0; i < value.Length; i += format.BytesPerSample)
                {
                    yield return new Sample(value.AsMemory(i, format.BytesPerSample).ToArray(), new SampleDefinition(format.Type, format.BitsPerSample, isLittleEndian));
                }
            }
        }
    }
}
