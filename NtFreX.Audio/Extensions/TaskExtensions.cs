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
        public static async Task<WaveAudioContainer> ToInMemoryContainerAsync([NotNull] this Task<WaveAudioContainerStream> audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var data = await audio.ConfigureAwait(false);
            return await data.ToContainerAsync(new MemoryStream(), cancellationToken).ConfigureAwait(false);
        }

        [return: NotNull]
        public static async Task<WaveAudioContainer> ToFileAsync([NotNull] this Task<WaveAudioContainerStream> audio, [NotNull] string path, [NotNull] FileMode fileMode = FileMode.CreateNew, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var data = await audio.ConfigureAwait(false);
            return await data.ToContainerAsync(path, fileMode, cancellationToken).ConfigureAwait(false);
        }

        [return: MaybeNull]
        public static async Task<TOutput> CastAsync<TSource, TOutput>([NotNull] this Task<TSource> task)
        {
            _ = task ?? throw new ArgumentNullException(nameof(task));

            return (TOutput) (object) await task.ConfigureAwait(false);
        }

        [return: NotNull]
        public static async Task DisposeAsync<T>([NotNull] Task<T> task)
            where T: IDisposable
        {
            var value = await task.ConfigureAwait(false);
            value.Dispose();
        }
    }
}
