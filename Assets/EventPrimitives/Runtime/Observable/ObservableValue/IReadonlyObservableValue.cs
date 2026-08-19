
namespace CatCode.Observables
{
    public interface IReadonlyObservableValue<T> : IReadonlyObservableSource<T>
    {
        T Value { get; }
    }
}