using Domain;

namespace Ports
{
    public interface IEventStoreStreamMessageReceiver
    {
        IDomainEvent Receive(DomainEventBuilder domainEventBuilder);
    }
}