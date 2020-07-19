using NtFreX.Audio.Containers;
using NtFreX.Audio.Samplers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class TaskExtensions
    {
        [return: NotNull]
        public static async Task<WaveStreamAudioContainer> ToInMemoryContainerAsync([NotNull] this Task<WaveEnumerableAudioContainer> audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var data = await audio.ConfigureAwait(false);
            return await data.ToInMemoryContainerAsync(cancellationToken).ConfigureAwait(false);
        }

        [return: NotNull]
        public static async Task<WaveStreamAudioContainer> ToFileAsync([NotNull] this Task<WaveEnumerableAudioContainer> audio, [NotNull] string path, [NotNull] FileMode fileMode = FileMode.CreateNew, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var data = await audio.ConfigureAwait(false);
            return await WaveEnumerableAudioContainerExtensions.ToFileAsync(data, path, fileMode, cancellationToken).ConfigureAwait(false);
        }

        [return: NotNull]
        public static async Task<WaveEnumerableAudioContainer> LogProgress([NotNull] this Task<WaveEnumerableAudioContainer> audio, [NotNull] Action<double> onProgress, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var data = await audio.ConfigureAwait(false);
            return data.LogProgress(onProgress, cancellationToken);
        }

        [return: MaybeNull]
        public static async Task<TOutput?> CastAsync<TSource, TOutput>([NotNull] this Task<TSource> task)
            where TOutput: class
        {
            _ = task ?? throw new ArgumentNullException(nameof(task));
            return await task.ConfigureAwait(false) as TOutput;
        }

        [return: NotNull]
        public static async Task DisposeAsync<T>([NotNull] Task<T> task)
            where T: IDisposable
        {
            _ = task ?? throw new ArgumentNullException(nameof(task));

            var value = await task.ConfigureAwait(false);
            value.Dispose();
        }
    }
}
