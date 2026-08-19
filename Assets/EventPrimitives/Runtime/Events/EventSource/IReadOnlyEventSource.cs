using System;

namespace CatCode.EventPrimitives
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