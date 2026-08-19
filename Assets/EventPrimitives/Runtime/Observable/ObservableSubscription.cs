using CatCode.Collections;
using System;

namespace CatCode.Observables
{
    public readonly struct ObservableSubscription : IDisposable
    {
        private readonly SlotId _id;
        private readonly ISlotStorage _storage;

        public ObservableSubscription(SlotId id, ISlotStorage storage)
        {
            _id = id;
            _storage = storage;
        }

        public readonly bool IsValid => _storage != null && _storage.IsValid(_id);
        public readonly bool IsDisposed => _storage == null || !_storage.IsValid(_id);

        public readonly void Unsubscribe()
            => _storage?.Remove(_id);

        public readonly void Dispose()
            => _storage?.Remove(_id);
    }
}