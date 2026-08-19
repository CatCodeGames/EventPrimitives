using System;

namespace CatCode.EventPrimitives
{
    public static class EventSourceExtensions
    {
        public static IDisposable AddListenerDisposable(this IReadOnlyEventSource eventSource, Action handler, bool invokeImmediately)
        {
            eventSource.Raised += handler;
            var subscription = new EventSourceSubscription(eventSource, handler);
            if (invokeImmediately)
                handler();
            return subscription;
        }

        public static EventSourceSubscriptionToken AddListenerScoped<T>(this IReadOnlyEventSource eventSource, Action handler, bool invokeImmediately)
        {
            eventSource.Raised += handler;
            var subscription = new EventSourceSubscriptionToken(eventSource, handler);
            if (invokeImmediately)
                handler();
            return subscription;
        }

        public static void AddListener(this IReadOnlyEventSource eventSource, Action handler, bool invokeImmediately)
        {
            eventSource.Raised += handler;
            if (invokeImmediately)
                handler();
        }

        public static void RemoveListener(this IReadOnlyEventSource eventSource, Action handler)
            => eventSource.Raised -= handler;
    }
}
