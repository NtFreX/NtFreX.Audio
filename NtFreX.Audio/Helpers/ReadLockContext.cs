using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Helpers
{
    public sealed class ReadLockContext<T> : IDisposable
        where T: class
    {
        private readonly SemaphoreSlim semaphore;

        public T? Data { [return:MaybeNull] get; }

        public ReadLockContext([NotNull] SemaphoreSlim semaphore, [MaybeNull] T? data)
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
