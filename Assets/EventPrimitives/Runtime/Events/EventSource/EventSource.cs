using System;

namespace CatCode.Events
{
    public sealed class EventSource : IEventSource
    {
        public event Action Raised;

        public void Raise() => Raised?.Invoke();
    }

    public sealed class EventSource<T> : IEventSource<T>
    {
        public event Action<T> Raised;

        public void Raise(T value) => Raised?.Invoke(value);
    }
}