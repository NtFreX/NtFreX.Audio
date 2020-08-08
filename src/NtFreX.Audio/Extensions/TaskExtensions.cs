using NtFreX.Audio.Containers;
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
        public static async Task<WaveEnumerableAudioContainer> LogProgressAsync([NotNull] this Task<WaveEnumerableAudioContainer> audio, [NotNull] Action<double> onProgress, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var data = await audio.ConfigureAwait(false);
            return data.LogProgress(onProgress, cancellationToken);
        }
    }
}
