using System;

namespace CatCode.Observables
{
    public interface IReadonlyObservableSource
    {
        ObservableSubscription Subscribe(Action action);
    }

    public interface IReadonlyObservableSource<T>
    {
        ObservableSubscription Subscribe(Action<T> action);
    }
}