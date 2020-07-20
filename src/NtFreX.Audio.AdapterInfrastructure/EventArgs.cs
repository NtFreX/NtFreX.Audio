using System;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public class EventArgs<T> : EventArgs
    {
        public T Value { get; }

        public EventArgs(T value)
        {
            Value = value;
        }
    }
}
