namespace CatCode.Collections
{
    public interface ISlotStorage
    {
        bool IsValid(SlotId itemId);
        bool Remove(SlotId itemId);
    }

    public interface ISlotStorage<T> : ISlotStorage
    {
        SlotId Add(T item);
    }
}