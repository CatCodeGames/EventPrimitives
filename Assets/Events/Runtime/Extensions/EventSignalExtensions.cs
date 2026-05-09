using System;

namespace CatCode.Events
{
    public static class EventSignalExtensions
    {
        public static IDisposable AddListenerDisposable(this IReadOnlyEventSignal eventSignal, Action handler, bool invokeImmediately)
        {
            eventSignal.Raised += handler;
            var subscription = new EventSignalSubscription(eventSignal, handler);
            if (invokeImmediately)
                handler();
            return subscription;
        }

        public static EventSignalSubscriptionToken AddListenerScoped<T>(this IReadOnlyEventSignal eventSignal, Action handler, bool invokeImmediately)
        {
            eventSignal.Raised += handler;
            var subscription = new EventSignalSubscriptionToken(eventSignal, handler);
            if (invokeImmediately)
                handler();
            return subscription;
        }

        public static void AddListener(this IReadOnlyEventSignal eventSignal, Action handler, bool invokeImmediately)
        {
            eventSignal.Raised += handler;
            if (invokeImmediately)
                handler();
        }

        public static void RemoveListener(this IReadOnlyEventSignal eventSignal, Action handler)
            => eventSignal.Raised -= handler;
    }
}
