using System;

namespace CatCode.EventPrimitives
{
    public sealed class EventValueSubscription<T> : IDisposable
    {
        private IReadOnlyEventValue<T> _owner;
        private Action<T> _handler;
                
        public EventValueSubscription(IReadOnlyEventValue<T> owner, Action<T> handler)
        {
            _owner = owner;
            _handler = handler;

            if (_owner != null && _handler != null)
                _owner.Changed += handler;
        }

        public void Dispose()
        {
            if (_owner != null && _handler != null)
            {
                _owner.Changed -= _handler;
                _owner = null;
                _handler = null;
            }
        }
    }
}
