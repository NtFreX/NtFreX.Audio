using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class IntermediateEnumerableAudioContainerExtensions
    {
        public static IntermediateEnumerableAudioContainer LogProgress(this IntermediateEnumerableAudioContainer audio, Action<double> onProgress, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var modifier = audio.GetDataLength();
            return new IntermediateEnumerableAudioContainer(
                audio.ForEachAsync((index, _) => onProgress.Invoke((index + 1) / modifier), cancellationToken),
                audio.GetFormat(),
                audio.IsDataLittleEndian());
        }

        public static async Task<IntermediateListAudioContainer> ToInMemoryContainerAsync(this Task<IntermediateEnumerableAudioContainer> container, CancellationToken cancellationToken = default)
        {
            _ = container ?? throw new ArgumentNullException(nameof(container));

            return await (await container.ConfigureAwait(false)).ToInMemoryContainerAsync(cancellationToken).ConfigureAwait(false);
        }

        public static async Task<IntermediateListAudioContainer> ToInMemoryContainerAsync(this IntermediateEnumerableAudioContainer container, CancellationToken cancellationToken = default)
        {
            _ = container ?? throw new ArgumentNullException(nameof(container));

            return new IntermediateListAudioContainer(
                await container.ToArrayAsync(cancellationToken).ConfigureAwait(false), 
                container.GetFormat(), 
                container.IsDataLittleEndian());
        }
    }
}
