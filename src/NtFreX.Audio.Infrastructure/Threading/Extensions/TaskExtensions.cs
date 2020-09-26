using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class TaskExtensions
    {
        [return: NotNull]
        public static async Task IgnoreCancelationError([NotNull] this Task task)
        {
            _ = task ?? throw new ArgumentNullException(nameof(task));

            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                /* IGNORE */
            }
        }

        [return: MaybeNull]
        public static async Task<TOutput?> CastAsync<TSource, TOutput>([NotNull] this Task<TSource> task)
            where TOutput : class
        {
            _ = task ?? throw new ArgumentNullException(nameof(task));
            return await task.ConfigureAwait(false) as TOutput;
        }

        [return: NotNull]
        public static async Task DisposeAsync<T>([NotNull] this Task<T> task)
            where T : IDisposable
        {
            _ = task ?? throw new ArgumentNullException(nameof(task));

            var value = await task.ConfigureAwait(false);
            value.Dispose();
        }
    }
}
