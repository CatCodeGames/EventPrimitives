using System;

namespace CatCode.Events
{
    public interface IReadOnlyEventSource
    {
        event Action Raised;
    }

    public interface IReadOnlyEventSource<T>
    {
        event Action<T> Raised;
    }
}