using System;

namespace CatCode.EventPrimitives
{
    public struct EventValueSubscriptionToken<T> : IDisposable
    {
        private IReadOnlyEventValue<T> _owner;
        private Action<T> _handler;

        public EventValueSubscriptionToken(IReadOnlyEventValue<T> owner, Action<T> handler)
        {
            _owner = owner;
            _handler = handler;
        }

        public void Dispose()
        {
            if (_owner == null || _handler == null)            
                return;
            
            _owner.Changed -= _handler;
            _owner = null;
            _handler = null;
        }
    }
}
