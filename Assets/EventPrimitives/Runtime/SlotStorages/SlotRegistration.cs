

using System;

namespace CatCode.Collections
{
    public readonly struct SlotRegistration : IDisposable
    {
        private readonly SlotId _id;
        private readonly ISlotStorage _storage;

        public SlotRegistration(SlotId id, ISlotStorage storage)
        {
            _id = id;
            _storage = storage;
        }

        public bool IsValid => _storage != null && _storage.IsValid(_id);
        
        public void Dispose()
        {
            _storage?.Remove(_id);
        }
    }
}