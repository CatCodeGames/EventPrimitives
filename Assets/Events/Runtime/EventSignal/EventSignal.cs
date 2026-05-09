using System;

namespace CatCode.Events
{
    public sealed class EventSignal : IEventSignal
    {
        public event Action Raised;

        public void Raise() => Raised?.Invoke();
    }
}