using CatCode.Collections;
using System;

namespace CatCode.Observables
{
    public sealed class ArrayBackedLinkedListSubscriberStorage : ISubscriberStorage
    {
        private readonly ArrayBackedLinkedList<Action> _storage;
        private int _publishDepth = 0;

        public ArrayBackedLinkedListSubscriberStorage(int capacity)
            => _storage = new(capacity);

        public SlotId Add(Action action)
            => _storage.Add(action);

        public bool Remove(SlotId itemId)
            => _publishDepth == 0
                ? _storage.Remove(itemId)
                : _storage.RemoveDeferred(itemId);

        public bool IsValid(SlotId itemId)
            => _storage.IsValid(itemId);

        public void Publish()
        {
            _publishDepth++;
            try
            {
                foreach (var callback in _storage)
                    callback();
            }
            finally
            {
                _publishDepth--;
                _storage.ApplyRemove();
            }
        }
    }

    public sealed class ArrayBackedLinkedListSubscriberStorage<T> : ISubscriberStorage<T>
    {
        private readonly ArrayBackedLinkedList<Action<T>> _storage;
        private int _publishDepth = 0;

        public ArrayBackedLinkedListSubscriberStorage(int capacity = 1)
            => _storage = new(capacity);

        public SlotId Add(Action<T> action)
            => _storage.Add(action);

        public bool Remove(SlotId itemId)
            => _publishDepth == 0
                ? _storage.Remove(itemId)
                : _storage.RemoveDeferred(itemId);

        public bool IsValid(SlotId itemId)
            => _storage.IsValid(itemId);

        public void Publish(T value)
        {
            _publishDepth++;
            try
            {
                foreach (var callback in _storage)
                    callback(value);
            }
            finally
            {
                _publishDepth--;
                if (_publishDepth == 0)
                    _storage.ApplyRemove();
            }
        }
    }
}