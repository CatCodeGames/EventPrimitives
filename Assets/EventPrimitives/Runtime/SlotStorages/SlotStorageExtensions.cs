namespace CatCode.Collections
{

    public static class SlotStorageExtensions
    {
        public static SlotRegistration Register<T>(this ISlotStorage<T> storage, T item)
        {
            var itemId = storage.Add(item);
            return new SlotRegistration(itemId, storage);
        }
    }
}