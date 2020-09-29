using NtFreX.Audio.Containers.Wave;
using NtFreX.Audio.Infrastructure.Container;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Converters
{
    internal class IntermediateToWaveConverter : AudioConverter<IIntermediateAudioContainer, WaveAudioContainer>
    {
        protected override Task<WaveAudioContainer> ConvertAsync(IIntermediateAudioContainer from, CancellationToken cancellationToken = default)
        {
            var format = from.GetFormat();
            var size = from.GetDataLength() * format.BytesPerSample;

            var container = WaveAudioContainerBuilder.Build(
                from.GetAsyncAudioEnumerable(cancellationToken),
                (uint) size,
                format,
                from.IsDataLittleEndian());

            return Task.FromResult(container);
        }
    }
}
