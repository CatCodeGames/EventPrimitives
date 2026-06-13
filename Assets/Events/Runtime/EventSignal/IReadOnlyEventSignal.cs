using System;

namespace CatCode.Events
{
    public interface IReadOnlyEventSignal
    {
        event Action Raised;
    }

    public interface IReadOnlyEventSignal<T>
    {
        event Action<T> Raised;
    }
}