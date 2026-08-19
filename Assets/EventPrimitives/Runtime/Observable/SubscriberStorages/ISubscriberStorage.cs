using CatCode.Collections;
using System;

namespace CatCode.EventPrimitives
{
    public interface ISubscriberStorage : ISlotStorage<Action>
    {
        void Publish();
    }
    
    public interface ISubscriberStorage<T> : ISlotStorage<Action<T>>
    {
        void Publish(T value);
    }
}