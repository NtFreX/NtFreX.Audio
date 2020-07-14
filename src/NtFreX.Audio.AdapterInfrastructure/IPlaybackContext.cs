using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IPlaybackContext : IDisposable 
    {
        Observable<EventArgs> EndOfDataReached { get; }
    }

    public class Observable<T>
        where T: EventArgs
    {
        private readonly List<Action<object, T>> handlers = new List<Action<object, T>>();
        private Stack<TaskCompletionSource<(object Sender, T Args)>> nextInvoke = new Stack<TaskCompletionSource<(object Sender, T Args)>>();

        public Observable() 
        {
            nextInvoke.Push(new TaskCompletionSource<(object Sender, T Args)>());
        }

        public void Subscribe(Action<object, T> handler)
        {
            handlers.Add(handler);
        }

        public void Unsubscribe(Action<object, T> handler)
        {
            handlers.Remove(handler);
        }

        public void Invoke(object obj, T args)
        {
            nextInvoke.Pop().SetResult((obj, args));
            nextInvoke.Push(new TaskCompletionSource<(object Sender, T Args)>());

            foreach (var handler in handlers)
            {
                handler.Invoke(obj, args);
            }
        }

        public Task<(object Sender, T Args)> WaitForNextEvent()
            => nextInvoke.Peek().Task;
    }
}
