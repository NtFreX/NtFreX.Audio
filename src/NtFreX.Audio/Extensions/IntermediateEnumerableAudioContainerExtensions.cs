using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using NtFreX.Audio.Samplers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class IntermediateEnumerableAudioContainerExtensions
    {
        public static Task<TTarget> ConvertAsync<TTarget>(this Task<IntermediateEnumerableAudioContainer> container, CancellationToken cancellationToken = default)
            where TTarget : IAudioContainer
            => container.ConvertAsync<IntermediateEnumerableAudioContainer, TTarget>(cancellationToken);

        public static async Task<IntermediateEnumerableAudioContainer> RunAudioPipeAsync(this Task<IntermediateEnumerableAudioContainer> containerTask, AudioSamplerPipe pipe, CancellationToken cancellationToken = default)
        {
            _ = containerTask ?? throw new ArgumentNullException(nameof(containerTask));
            _ = pipe ?? throw new ArgumentNullException(nameof(pipe));

            var container = await containerTask.ConfigureAwait(false);
            return await RunAudioPipeAsync(container, pipe, cancellationToken).ConfigureAwait(false);
        }

        public static Task<IntermediateEnumerableAudioContainer> RunAudioPipeAsync(this IntermediateEnumerableAudioContainer container, AudioSamplerPipe pipe, CancellationToken cancellationToken = default)
        {
            _ = container ?? throw new ArgumentNullException(nameof(container));
            _ = pipe ?? throw new ArgumentNullException(nameof(pipe));

            return pipe.RunAsync(container, cancellationToken);
        }

        public static async Task<IntermediateEnumerableAudioContainer> LogProgressAsync(this Task<IntermediateEnumerableAudioContainer> audio, Func<double, Task> onProgress, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var data = await audio.ConfigureAwait(false);
            return data.LogProgress(onProgress, cancellationToken);
        }

        public static async Task<IntermediateEnumerableAudioContainer> LogProgressAsync(this Task<IntermediateEnumerableAudioContainer> audio, Action<double> onProgress, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var data = await audio.ConfigureAwait(false);
            return data.LogProgress(onProgress, cancellationToken);
        }

        public static IntermediateEnumerableAudioContainer LogProgress(this IntermediateEnumerableAudioContainer audio, Func<double, Task> onProgress, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            double modifier = audio.GetDataLength();
            return new IntermediateEnumerableAudioContainer(
                audio.ForEachAsync((index, _) => onProgress.Invoke((index + 1) / modifier), cancellationToken),
                audio.GetFormat(),
                audio.IsDataLittleEndian());
        }

        public static IntermediateEnumerableAudioContainer LogProgress(this IntermediateEnumerableAudioContainer audio, Action<double> onProgress, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            double modifier = audio.GetDataLength();
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
