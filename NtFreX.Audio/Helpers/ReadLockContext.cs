using System;
using System.Threading;

namespace NtFreX.Audio.Helpers
{
    public class ReadLockContext<T> : IDisposable
    {
        private readonly SemaphoreSlim semaphore;

        public T Data { get; }

        public ReadLockContext(SemaphoreSlim semaphore, T data)
        {
            Data = data;
            this.semaphore = semaphore;
        }

        public void Dispose()
        {
            semaphore.Release();
        }
    }
}
