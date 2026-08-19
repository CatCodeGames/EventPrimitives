using System;
using System.Collections.Generic;

namespace CatCode.Observables
{
    public sealed class ObservableValue<T> : IObservableValue<T>
    {
        private T _value;
        private readonly ISubscriberStorage<T> _storage;
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
                _storage.Publish(value);
            }
        }

        public ObservableValue(T initial, ISubscriberStorage<T> storage, NotificationMode notifyMode = NotificationMode.OnChange, IEqualityComparer<T> comparer = null)
        {
            _value = initial;
            _notifyOnlyOnChange = notifyMode == NotificationMode.OnChange;
            _storage = storage;
            _comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public ObservableSubscription Subscribe(Action<T> action)
            => new(_storage.Add(action), _storage);

        public void SetSilently(T value)
            => _value = value;

        public void SetAndNotify(T value)
        {
            _value = value;
            _storage.Publish(value);
        }

        public void ForceNotify()
            => _storage.Publish(_value);

        public override string ToString()
            => _value?.ToString() ?? "null";

        public static ObservableValue<T> CreateDefault(T initial, NotificationMode notifyMode = NotificationMode.OnChange, IEqualityComparer<T> comparer = null)
            => new(initial, new ArrayBackedLinkedListSubscriberStorage<T>(4), notifyMode, comparer);
    }
}