using System;
using UnityEngine.Events;

namespace CatCode.Events
{
    public sealed class UnityEventValueAdapter<T> : IReadOnlyEventValue<T>
    {
        private readonly UnityEvent<T> _unityEvent;
        private T _value;
        private bool _isDisposed;

        public event Action<T> Changed;

        public T Value => _value;
        public bool IsDisposed => _isDisposed;

        public UnityEventValueAdapter(UnityEvent<T> unityEvent, T initialValue = default)
        {
            _unityEvent = unityEvent;
            _value = initialValue;

            _unityEvent.AddListener(EventHandler);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;
            _unityEvent?.RemoveListener(EventHandler);
        }

        private void EventHandler(T value)
        {
            _value = value;
            Changed?.Invoke(_value);
        }
    }
}
