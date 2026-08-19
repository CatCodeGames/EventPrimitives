using CatCode.Collections;
using System;

namespace CatCode.Observables
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