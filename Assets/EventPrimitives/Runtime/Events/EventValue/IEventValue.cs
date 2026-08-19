namespace CatCode.EventPrimitives
{
    public interface IEventValue<T> : IReadOnlyEventValue<T>
    {
        new T Value { get; set; }
        void SetSilent(T value);
        void ForceNotify();
    }
}
