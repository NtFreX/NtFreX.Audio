using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Helpers
{
    public class ReadLock<T> : IDisposable 
        where T : class, IDisposable
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private readonly T? data;
        private readonly Action<T?>? aquireAction;
        private bool isDisposed;

        internal ReadLock([MaybeNull] T? data, [MaybeNull] Action<T?>? aquireAction)
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

        public T? UnsafeAccess() => data;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeData)
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            semaphore.Dispose();

            if (disposeData)
            {
                data?.Dispose();
            }
        }
    }
}
