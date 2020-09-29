using System;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading.Extensions
{
    public static class TaskExtensions
    {
        public static async Task IgnoreCancelationError(this Task task)
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

        public static async Task<TOutput> CastAsync<TSource, TOutput>(this Task<TSource> task)
            where TOutput : class
        {
            _ = task ?? throw new ArgumentNullException(nameof(task));
            var casted = await task.ConfigureAwait(false) as TOutput;
            return casted ?? throw new InvalidCastException();
        }

        public static async Task DisposeAsync<T>(this Task<T> task)
            where T : IDisposable
        {
            _ = task ?? throw new ArgumentNullException(nameof(task));

            var value = await task.ConfigureAwait(false);
            value.Dispose();
        }
    }
}
