using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure
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
    }
}
