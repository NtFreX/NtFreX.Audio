using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Helpers
{
    public class ReadLock<T> : IDisposable 
        where T : class, IDisposable
    {
        private readonly object locking = new object();
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private readonly T? data;
        private readonly Action<T?>? aquireAction;
        private bool isDisposed;

        internal ReadLock([MaybeNull] T? data, [MaybeNull] Action<T?>? aquireAction)
        {
            this.data = data;
            this.aquireAction = aquireAction;
        }

        public async Task<ReadLockContext<T>> AquireAsync(CancellationToken cancellationToken = default)
        {
            bool wasLockTaken = false;
            try 
            {
                Monitor.Enter(locking, ref wasLockTaken);
                await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                aquireAction?.Invoke(data);
                return new ReadLockContext<T>(semaphore, data);
            }
            finally
            {
                if(wasLockTaken)
                {
                    Monitor.Exit(locking);
                }
            }
        }

        public bool TryAquire(out ReadLockContext<T>? context)
        {
            lock (locking) 
            {
                if (semaphore.CurrentCount <= 0)
                {
                    context = null;
                    return false;
                }

                semaphore.Wait();
                aquireAction?.Invoke(data);
                context = new ReadLockContext<T>(semaphore, data);
                return true;
            }
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
