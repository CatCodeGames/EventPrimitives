namespace CatCode.Observables
{
    public interface IObservableSource : IReadonlyObservableSource
    {
        void Publish();
    }

    public interface IObservableSource<T> : IReadonlyObservableSource<T>
    {
        void Publish(T value);
    }
}