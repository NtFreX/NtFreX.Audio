using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    // TODO: why is IAsyncEnumerator .Current not nullable?
    internal sealed class AsyncEnumeratorWrapper<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> enumerator;
        private readonly CancellationToken cancellationToken;

        public AsyncEnumeratorWrapper(IEnumerator<T> enumerator, CancellationToken cancellationToken)
        {
            this.enumerator = enumerator;
            this.cancellationToken = cancellationToken;
        }

        public T Current => enumerator.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                return new ValueTask<bool>(enumerator.MoveNext());
            }
            catch (Exception ex)
            {
                // TODO: catch exceptions in other implementations
                var tcs = new TaskCompletionSource<bool>();
                tcs.SetException(ex);
                return new ValueTask<bool>(tcs.Task);
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
