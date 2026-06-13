namespace CatCode.Events
{
    public interface IEventSignal : IReadOnlyEventSignal
    {
        void Raise();
    }

    public interface IEventSignal<T> : IReadOnlyEventSignal<T>
    {
        void Raise(T value);
    }
}