using NtFreX.Audio.Containers;
using NtFreX.Audio.Containers.Wave;
using NtFreX.Audio.Infrastructure;
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
            var seekableSamples = from.DataSubChunk.ToSamplesAsync(from.GetByteLength(), format, isLittleEndian, cancellationToken);

            var container = new IntermediateEnumerableAudioContainer(
                seekableSamples,
                format,
                isLittleEndian);
            return Task.FromResult(container);
        }
    }
}
