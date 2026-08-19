namespace CatCode.Observables
{
    public interface IObservableValue<T> : IReadonlyObservableValue<T>
    {
        new T Value { get; set; }
    }
}