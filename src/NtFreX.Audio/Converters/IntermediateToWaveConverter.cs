using NtFreX.Audio.Containers;
using NtFreX.Audio.Containers.Wave;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Infrastructure.Threading;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Converters
{
    internal class IntermediateToWaveConverter : AudioConverter<IntermediateEnumerableAudioContainer, WaveAudioContainer>
    {
        protected override Task<WaveAudioContainer> ConvertAsync(IntermediateEnumerableAudioContainer from, CancellationToken cancellationToken = default)
        {
            var format = from.GetFormat();
            var size = from.GetDataLength() * format.BytesPerSample;

            var container = WaveAudioContainerBuilder.Build(
                new SeekableAsyncEnumerable<IReadOnlyList<byte>>(
                    cancellationToken => from.GetAsyncAudioEnumerator(cancellationToken),
                    size),
                (uint) size,
                format,
                from.IsDataLittleEndian());

            return Task.FromResult(container);
        }
    }
}
