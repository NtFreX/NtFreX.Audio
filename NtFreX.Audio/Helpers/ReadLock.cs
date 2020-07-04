using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Helpers
{
    public class ReadLock<T> : IDisposable where T : class
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private readonly T data;
        private readonly Action<T> aquireAction;

        internal ReadLock(T data, Action<T> aquireAction)
        {
            this.data = data;
            this.aquireAction = aquireAction;
        }

        public T AquireAndDisposeOrThrow()
        {
            if (semaphore.Wait(TimeSpan.Zero))
            {
                Dispose();
                aquireAction?.Invoke(data);
                return data;
            }
            else
            {
                throw new Exception("The data is allready aquired elsewhere");
            }
        }

        public async Task<ReadLockContext<T>> AquireAsync(CancellationToken cancellationToken = default)
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            aquireAction?.Invoke(data);
            return new ReadLockContext<T>(semaphore, data);
        }

        public void Dispose()
        {
            semaphore.Dispose();
        }
    }
}
