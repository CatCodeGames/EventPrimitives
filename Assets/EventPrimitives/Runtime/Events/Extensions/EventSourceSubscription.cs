using System;

namespace CatCode.Events
{
    public sealed class EventSourceSubscription : IDisposable
    {
        private IReadOnlyEventSource _owner;
        private Action _handler;

        public EventSourceSubscription(IReadOnlyEventSource owner, Action handler)
        {
            _owner = owner;
            _handler = handler;

            if (_owner != null && _handler != null)
                _owner.Raised += handler;
        }

        public void Dispose()
        {
            if (_owner == null || _handler == null)
                return;

            _owner.Raised -= _handler;
            _owner = null;
            _handler = null;
        }
    }
}
