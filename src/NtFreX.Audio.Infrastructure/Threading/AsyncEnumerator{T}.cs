using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NtFreX.Audio.Infrastructure.Threading
{
    internal class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly Func<ValueTask<bool>> moveNextAction;
        private readonly Func<T> currentAccessor;
        private readonly Func<ValueTask> disposeAction;

        public T Current => currentAccessor();

        public AsyncEnumerator(Func<ValueTask<bool>> moveNextAction, Func<T> currentAccessor, Func<ValueTask> disposeAction)
        {
            this.moveNextAction = moveNextAction;
            this.currentAccessor = currentAccessor;
            this.disposeAction = disposeAction;
        }

        public ValueTask DisposeAsync()
            => disposeAction();

        public ValueTask<bool> MoveNextAsync()
            => moveNextAction();
    }
}
