using System;


namespace CatCode.Events
{
    public interface IReadOnlyEventValue<out T>
    {
        T Value { get; }
        event Action<T> Changed;
    }
}
