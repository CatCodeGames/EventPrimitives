using System;

namespace CatCode.Events
{
    public sealed class EventSignal : IEventSignal
    {
        public event Action Raised;

        public void Raise() => Raised?.Invoke();
    }

    public sealed class EventSignal<T> : IEventSignal<T>
    {
        public event Action<T> Raised;

        public void Raise(T value) => Raised?.Invoke(value);
    }
}