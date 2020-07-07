using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Helpers
{
    public class ReadLock<T> : IDisposable where T : class, IDisposable
    {
        private bool isDisposed = false;
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private readonly T data;
        private readonly Action<T> aquireAction;

        internal ReadLock([MaybeNull] T data, [MaybeNull] Action<T> aquireAction)
        {
            this.data = data;
            this.aquireAction = aquireAction;
        }

        [return:NotNull] public async Task<ReadLockContext<T>> AquireAsync([MaybeNull] CancellationToken cancellationToken = default)
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            aquireAction?.Invoke(data);
            return new ReadLockContext<T>(semaphore, data);
        }

        public void Dispose() => Dispose(true);

        public void Dispose(bool disposeData)
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            semaphore.Dispose();

            if (disposeData)
            {
                data.Dispose();
            }
        }
    }
}
