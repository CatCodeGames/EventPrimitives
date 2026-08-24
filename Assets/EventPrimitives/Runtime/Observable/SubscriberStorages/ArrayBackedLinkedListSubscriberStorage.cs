using CatCode.Collections;
using System;

namespace CatCode.EventPrimitives
{
    public sealed class ArrayBackedLinkedListSubscriberStorage : ISubscriberStorage
    {
        private readonly ArrayBackedLinkedList<Action> _storage;
        private int _publishDepth = 0;

        public ArrayBackedLinkedListSubscriberStorage(int capacity)
            => _storage = new(capacity);

        public SlotId Add(Action action)
            => _storage.Add(action);
        public Action Get(SlotId slotId)
            => _storage.Get(slotId);

        public bool TryGet(SlotId slotId, out Action item)
            => _storage.TryGet(slotId, out item);

        public bool Remove(SlotId slotId)
            => _publishDepth == 0
                ? _storage.Remove(slotId)
                : _storage.RemoveDeferred(slotId);

        public bool IsValid(SlotId slotId)
            => _storage.IsValid(slotId);

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
        
        public Action<T> Get(SlotId slotId)
            => _storage.Get(slotId);

        public bool TryGet(SlotId slotId, out Action<T> item)
            => _storage.TryGet(slotId, out item);

        public bool Remove(SlotId slotId)
            => _publishDepth == 0
                ? _storage.Remove(slotId)
                : _storage.RemoveDeferred(slotId);

        public bool IsValid(SlotId slotId)
            => _storage.IsValid(slotId);

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