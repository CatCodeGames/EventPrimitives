using System;
using System.Collections.Generic;


namespace CatCode.EventPrimitives
{
    public sealed class EventValue<T> : IEventValue<T>
    {
        private T _value;
        private readonly IEqualityComparer<T> _comparer;
        private readonly bool _notifyOnlyOnChange;

        public T Value
        {
            get => _value;
            set
            {
                if (_notifyOnlyOnChange && _comparer.Equals(_value, value))
                    return;

                _value = value;
                Changed?.Invoke(_value);
            }
        }

        public event Action<T> Changed;

        public EventValue(T initial, NotificationMode notifyMode = NotificationMode.OnChange, IEqualityComparer<T> comparer = null)
        {
            _value = initial;
            _notifyOnlyOnChange = notifyMode == NotificationMode.OnChange;
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public void SetSilent(T value)
            => _value = value;

        public void ForceNotify()
            => Changed?.Invoke(_value);

        public override string ToString()
            => _value?.ToString() ?? "null";
    }
}
