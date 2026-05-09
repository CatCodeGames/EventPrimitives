using System;
using System.Collections.Generic;

namespace CatCode.Events
{
    public sealed class AggregatedEventValue<T> : IReadOnlyEventValue<T>, IDisposable
    {
        private readonly int _count;
        private readonly IReadOnlyEventValue<T>[] _sources;
        private readonly T[] _sourceValues;
        private readonly Action<T>[] _sourceHandlers;
        private readonly Func<T[], T> _aggregator;
        private readonly bool _notifyOnlyOnChange;
        private readonly EqualityComparer<T> _comparer;

        private T _value;
        private bool _isDisposed;

        public T Value => _value;

        public event Action<T> Changed;

        public AggregatedEventValue(IReadOnlyEventValue<T>[] sources, Func<T[], T> aggregator, bool notifyOnlyOnChange = true, EqualityComparer<T> comparer = null)
        {
            _count = sources.Length;

            _sources = sources;
            _aggregator = aggregator;
            _notifyOnlyOnChange = notifyOnlyOnChange;
            _comparer = comparer ?? EqualityComparer<T>.Default;

            _sourceValues = new T[_count];
            _sourceHandlers = new Action<T>[_count];

            for (int i = 0; i < _count; i++)
            {
                var index = i;
                _sourceValues[i] = sources[i].Value;
                Action<T> handler = (value) => EventHandler(value, index);
                _sourceHandlers[i] = handler;
                _sources[index].Changed += handler;
            }

            _value = aggregator(_sourceValues);
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            _isDisposed = true;

            for (int i = 0; i < _count; i++)
                _sources[i].Changed -= _sourceHandlers[i];
        }

        private void EventHandler(T value, int index)
        {            
            if (_notifyOnlyOnChange && _comparer.Equals(_sourceValues[index], value))
                return;

            _sourceValues[index] = value;

            var newValue = _aggregator(_sourceValues);

            if (_notifyOnlyOnChange && _comparer.Equals(_value, newValue))
                return;

            _value = newValue;
            Changed?.Invoke(_value);
        }
    }
}
