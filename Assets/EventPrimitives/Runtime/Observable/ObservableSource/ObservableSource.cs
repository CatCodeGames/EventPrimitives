using System;

namespace CatCode.EventPrimitives
{
    public sealed class ObservableSource : IObservableSource
    {
        private readonly ISubscriberStorage _storage;

        public ObservableSource(ISubscriberStorage storage)
            => _storage = storage;

        public void Publish()
            => _storage.Publish();

        public ObservableSubscription Subscribe(Action action)
            => new(_storage.Add(action), _storage);

        public static ObservableSource CreateDefault()
            => new(new ArrayBackedLinkedListSubscriberStorage(4));
    }

    public sealed class ObservableSource<T> : IObservableSource<T>
    {
        private readonly ISubscriberStorage<T> _storage;

        public ObservableSource(ISubscriberStorage<T> storage)
            => _storage = storage;

        public void Publish(T value)
            => _storage.Publish(value);

        public ObservableSubscription Subscribe(Action<T> action)
            => new(_storage.Add(action), _storage);

        public static ObservableSource<T> CreateDefault()
            => new(new ArrayBackedLinkedListSubscriberStorage<T>(4));
    }
}