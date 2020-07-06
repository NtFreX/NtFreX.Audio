using NtFreX.Audio.Containers;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class TaskExtensions
    {
        [return: NotNull]
        public static async Task<AudioContainer> ToFileAsync<T>([NotNull] this Task<T> audio, [NotNull] string path, [MaybeNull] CancellationToken cancellationToken = default)
            where T : AudioContainer
        {
            var container = await audio.ConfigureAwait(false);
            await container.ToFileAsync(path, cancellationToken).ConfigureAwait(false);
            return container;
        }

        [return: MaybeNull]
        public static async Task<TOutput> CastAsync<TSource, TOutput>([NotNull] this Task<TSource> task)
        {
            return (TOutput) (object) await task.ConfigureAwait(false);
        }
    }
}
