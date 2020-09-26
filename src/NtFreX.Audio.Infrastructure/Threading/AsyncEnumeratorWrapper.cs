using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    // TODO: why is IAsyncEnumerator .Current not nullable?
    internal sealed class AsyncEnumeratorWrapper<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> enumerator;

        public AsyncEnumeratorWrapper(IEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
        }

        public T Current => enumerator.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            try
            {
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
