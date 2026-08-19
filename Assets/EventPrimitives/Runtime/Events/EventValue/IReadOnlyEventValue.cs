using System;


namespace CatCode.EventPrimitives
{
    public interface IReadOnlyEventValue<out T>
    {
        T Value { get; }
        event Action<T> Changed;
    }
}
