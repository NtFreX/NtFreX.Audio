using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal sealed class AsyncEnumeratorWrapper<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> enumerator;
        private readonly bool runSynchronously;
        private readonly CancellationToken cancellationToken;

        public AsyncEnumeratorWrapper(IEnumerator<T> enumerator, bool runSynchronously, CancellationToken cancellationToken)
        {
            this.enumerator = enumerator;
            this.runSynchronously = runSynchronously;
            this.cancellationToken = cancellationToken;
        }

        public T Current => enumerator.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            if (runSynchronously)
            {
                try
                {
                    return new ValueTask<bool>(enumerator.MoveNext());
                }
                catch (Exception ex)
                {
                    var tcs = new TaskCompletionSource<bool>();
                    tcs.SetException(ex);
                    return new ValueTask<bool>(tcs.Task);
                }
            }
            else
            {
                return new ValueTask<bool>(Task.Run(() => enumerator.MoveNext(), cancellationToken));
            }
        }

        public void Dispose()
        {
            enumerator.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return default;
        }
    }
}
