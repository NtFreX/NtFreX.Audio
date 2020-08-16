using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public sealed class Observable<T> : IDisposable
        where T: EventArgs
    {
        private readonly List<Action<object, T>> handlers = new List<Action<object, T>>();
        private readonly List<Func<object, T, Task>> asyncHandlers = new List<Func<object, T, Task>>();
        private readonly List<TaskCompletionSource<(object Sender, T Args)>> nextInvokeHandlers = new List<TaskCompletionSource<(object Sender, T Args)>>();
        private readonly object myLock = new object();
        private bool isDisposed;

        public void Subscribe(Action<object, T> handler)
        {
            lock (myLock)
            {
                ThrowIfDisposed();

                handlers.Add(handler);
            }
        }

        public void Subscribe(Func<object, T, Task> handler)
        {
            lock (myLock)
            {
                ThrowIfDisposed();

                asyncHandlers.Add(handler);
            }
        }

        public void Unsubscribe(Action<object, T> handler)
        {
            lock (myLock)
            {
                ThrowIfDisposed();

                handlers.Remove(handler);
            }
        }

        public void Unsubscribe(Func<object, T, Task> handler)
        {
            lock (myLock)
            {
                ThrowIfDisposed();

                asyncHandlers.Remove(handler);
            }
        }

        public async Task InvokeAsync(object obj, T args)
        {
            if(!Monitor.TryEnter(myLock, -1))
            {
                throw new Exception("Failed to enter observable lock");
            }

            ThrowIfDisposed();

            try
            {
                foreach (var source in nextInvokeHandlers)
                {
                    source.TrySetResult((obj, args));
                }
                nextInvokeHandlers.Clear();

                foreach (var handler in handlers)
                {
                    handler.Invoke(obj, args);
                }
                foreach (var asyncHandler in asyncHandlers)
                {
                    await asyncHandler.Invoke(obj, args).ConfigureAwait(false);
                }
            }
            finally
            {
                Monitor.Exit(myLock);
            }
        }

        public Task<(object Sender, T Args)> WaitForNextEvent(CancellationToken cancellationToken = default)
        {
            lock (myLock)
            {
                ThrowIfDisposed();

                var source = new TaskCompletionSource<(object Sender, T Args)>();
                cancellationToken.Register(() => source.TrySetCanceled(cancellationToken));

                nextInvokeHandlers.Add(source);

                return source.Task;
            }
        }

        public void Dispose()
        {
            lock (myLock)
            {
                if(isDisposed)
                {
                    return;
                }

                foreach (var source in nextInvokeHandlers)
                {
                    source.TrySetCanceled();
                }
                nextInvokeHandlers.Clear();
                isDisposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(Observable<T>));
            }
        }
    }
}
