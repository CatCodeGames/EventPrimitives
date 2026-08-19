using System;
using System.Collections;
using System.Collections.Generic;


namespace CatCode.Collections
{
    public sealed class ArrayBackedLinkedList<T> : ISlotStorage<T>
    {
        private const int None = -1;

        public struct Enumerator : IEnumerator<T>
        {
            private readonly Slot[] _slots;
            private int _next;
            private int _current;
            private int _headIndex;

            public Enumerator(Slot[] slots, int headIndex)
            {
                _slots = slots;
                _headIndex = headIndex;
                _next = headIndex;
                _current = -1;
            }

            public T Current =>
                 _slots[_current].Item;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_next == -1)
                    return false;

                _current = _next;
                _next = _slots[_current].Next;

                return true;
            }

            public void Reset()
            {
                _next = _headIndex;
            }
        }

        public struct Slot
        {
            public T Item;
            public int Generation;
            public int Prev;
            public int Next;
            public int RemovalNext;
        }


        private Slot[] _slots;

        private int _headIndex = None;
        private int _tailIndex = None;

        private int _freeHeadIndex = None;

        private int _removalHeadIndex = None;
        private int _removalTailIndex = None;

        private int _count;

        public int Count => _count;

        public ArrayBackedLinkedList(int capacity = 4)
        {
            if (capacity < 1)
                capacity = 1;

            _slots = new Slot[capacity];

            for (int i = 0; i < capacity - 1; i++)
            {
                ref var slot = ref _slots[i];
                slot.Next = i + 1;
                slot.RemovalNext = None;
            }

            _slots[^1].Next = None;
            _freeHeadIndex = 0;
        }


        public bool IsValid(SlotId itemId)
            => _slots[itemId.Index].Generation == itemId.Generation;


        public SlotId Add(T item)
        {
            var index = AllocateSlot();

            ref var slot = ref _slots[index];

            slot.Item = item;
            slot.Prev = _tailIndex;
            slot.Next = None;

            if (_tailIndex == None)
                _headIndex = index;
            else
            {
                ref var tail = ref _slots[_tailIndex];
                tail.Next = index;
            }

            _tailIndex = index;
            _count++;

            return new SlotId(index, slot.Generation);
        }

        public bool Remove(SlotId itemId)
        {
            if (!IsValid(itemId))
                return false;

            int index = itemId.Index;
            ref var slot = ref _slots[index];

            UnlinkActive(ref slot);
            FreeSlot(index, ref slot);

            slot.Generation++;
            _count--;
            return true;
        }

        public bool RemoveDeferred(SlotId itemId)
        {
            if (!IsValid(itemId))
                return false;

            int index = itemId.Index;
            ref var slot = ref _slots[index];

            EnqueuePending(index, ref slot);

            slot.Generation++;
            _count--;
            return true;
        }

        public void ApplyRemove()
        {
            while (_removalHeadIndex != None)
            {
                int index = _removalHeadIndex;
                ref var slot = ref _slots[index];

                _removalHeadIndex = slot.RemovalNext;
                
                UnlinkActive(ref slot);
                FreeSlot(index, ref slot);
            }

            _removalTailIndex = None;
        }


        public T Get(SlotId itemId)
        {
            if (!IsValid(itemId))
                throw new ArgumentException("Invalid slot id.", nameof(itemId));

            return _slots[itemId.Index].Item;
        }

        public T GetUnchecked(SlotId itemId)
            => _slots[itemId.Index].Item;

        public ref T GetRef(SlotId itemId)
        {
            if (!IsValid(itemId))
                throw new ArgumentException("Invalid slot id.", nameof(itemId));

            return ref _slots[itemId.Index].Item;
        }

        public ref T GetRefUnchecked(SlotId itemId)
            => ref _slots[itemId.Index].Item;

        public bool TryGet(SlotId itemId, out T item)
        {
            if (!IsValid(itemId))
            {
                item = default;
                return false;
            }
            item = _slots[itemId.Index].Item;
            return true;
        }


        public Enumerator GetEnumerator()
            => new(_slots, _headIndex);

        private int AllocateSlot()
        {
            if (_freeHeadIndex == None)
                Resize();

            int index = _freeHeadIndex;
            _freeHeadIndex = _slots[index].Next;

            return index;
        }

        private void FreeSlot(int index, ref Slot slot)
        {
            slot.Item = default;
            slot.Next = _freeHeadIndex;
            slot.RemovalNext = None;
            _freeHeadIndex = index;
        }


        private void Resize()
        {
            int oldLength = _slots.Length;
            int newLength = oldLength * 2;

            Array.Resize(ref _slots, newLength);

            for (int i = oldLength; i < newLength - 1; i++)
                _slots[i].Next = i + 1;

            _slots[newLength - 1].Next = _freeHeadIndex;
            _freeHeadIndex = oldLength;
        }

        private void UnlinkActive(ref Slot slot)
        {
            if (slot.Prev == None)
                _headIndex = slot.Next;
            else
            {
                ref var prevSlot = ref _slots[slot.Prev];
                prevSlot.Next = slot.Next;
            }

            if (slot.Next == None)
                _tailIndex = slot.Prev;
            else
            {
                ref var nextSlot = ref _slots[slot.Next];
                _slots[slot.Next].Prev = slot.Prev;
            }
        }

        private void EnqueuePending(int index, ref Slot slot)
        {
            if (_removalTailIndex == None)
                _removalHeadIndex = index;
            else
                _slots[_removalTailIndex].RemovalNext = index;

            _removalTailIndex = index;
        }
    }
}