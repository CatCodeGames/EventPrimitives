using System;
using UnityEngine.Events;

namespace CatCode.Events
{
    public sealed class UnityEventSourceAdapter : IReadOnlyEventSource, IDisposable
    {
        private readonly UnityEvent _unityEvent;
        private bool _isDisposed;

        public event Action Raised;

        public bool IsDisposed => _isDisposed;

        public UnityEventSourceAdapter(UnityEvent unityEvent)
        {
            _unityEvent = unityEvent;
            _unityEvent?.AddListener(EventHandler);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;
            _unityEvent?.RemoveListener(EventHandler);
        }

        private void EventHandler()
            => Raised?.Invoke();
    }
}