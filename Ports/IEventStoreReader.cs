using Domain;

namespace Ports
{
    public interface IEventStoreReader
    {
        IEventStoreSubscription SubscribeToAllEvents(DomainEventPosition startPosition, IEventStoreStreamMessageReceiver receiver);
    }
}