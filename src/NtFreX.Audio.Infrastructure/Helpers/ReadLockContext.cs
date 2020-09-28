using System;
using System.Threading;

namespace NtFreX.Audio.Infrastructure.Helpers
{
    public sealed class ReadLockContext<T> : IDisposable
        where T: class
    {
        private readonly SemaphoreSlim semaphore;

        public T? Data { get; }

        public ReadLockContext(SemaphoreSlim semaphore, T? data)
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
