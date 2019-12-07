using EventStore.ClientAPI;
using Ports;

namespace EventStoreAdapter
{
    internal class EventStoreSubscription : IEventStoreSubscription
    {
        private readonly EventStoreCatchUpSubscription _subscription;

        public EventStoreSubscription(EventStoreCatchUpSubscription subscription)
        {
            _subscription = subscription;
        }
        
        public void Dispose()
        {
            _subscription?.Stop();
        }
    }
}