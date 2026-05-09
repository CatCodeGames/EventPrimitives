using System;

namespace CatCode.Events
{
    public static class EventValueExtensions
    {
        public static IDisposable AddListenerDisposable<T>(this IReadOnlyEventValue<T> eventValue, Action<T> handler, bool invokeImmediately)
        {
            eventValue.Changed += handler;
            var subscription = new EventValueSubscription<T>(eventValue, handler);
            if (invokeImmediately)
                handler(eventValue.Value);
            return subscription;
        }

        public static EventValueSubscriptionToken<T> AddListenerScoped<T>(this IReadOnlyEventValue<T> eventValue, Action<T> handler, bool invokeImmediately)
        {
            eventValue.Changed += handler;
            var subscription = new EventValueSubscriptionToken<T>(eventValue, handler);
            if (invokeImmediately)
                handler(eventValue.Value);
            return subscription;
        }

        public static void AddListener<T>(this IReadOnlyEventValue<T> eventValue, Action<T> handler, bool invokeImmediately)
        {
            eventValue.Changed += handler;
            if (invokeImmediately)
                handler(eventValue.Value);
        }

        public static void RemoveListener<T>(this IReadOnlyEventValue<T> eventValue, Action<T> handler)
            => eventValue.Changed -= handler;
    }
}
