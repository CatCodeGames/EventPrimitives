using System;

namespace CatCode.EventPrimitives
{
    public struct EventSourceSubscriptionToken : IDisposable
    {
        private IReadOnlyEventSource _owner;
        private Action _handler;

        public EventSourceSubscriptionToken(IReadOnlyEventSource owner, Action handler)
        {
            _owner = owner;
            _handler = handler;
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
