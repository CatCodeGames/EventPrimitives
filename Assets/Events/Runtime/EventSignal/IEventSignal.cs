namespace CatCode.Events
{
    public interface IEventSignal : IReadOnlyEventSignal
    {
        void Raise();
    }
}