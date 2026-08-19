namespace CatCode.EventPrimitives
{
    public interface IObservableValue<T> : IReadOnlyObservableValue<T>
    {
        new T Value { get; set; }
    }
}