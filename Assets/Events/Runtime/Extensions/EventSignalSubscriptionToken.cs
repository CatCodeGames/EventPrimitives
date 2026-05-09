using System;

namespace CatCode.Events
{
    public struct EventSignalSubscriptionToken : IDisposable
    {
        private IReadOnlyEventSignal _owner;
        private Action _handler;

        public EventSignalSubscriptionToken(IReadOnlyEventSignal owner, Action handler)
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
