namespace CatCode.EventPrimitives
{
    public interface IEventSource : IReadOnlyEventSource
    {
        void Raise();
    }

    public interface IEventSource<T> : IReadOnlyEventSource<T>
    {
        void Raise(T value);
    }
}