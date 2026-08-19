
namespace CatCode.EventPrimitives
{
    public interface IReadOnlyObservableValue<T> : IReadonlyObservableSource<T>
    {
        T Value { get; }
    }
}