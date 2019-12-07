using Domain;

namespace Ports
{
    public interface IEventStoreStreamMessageReceiver
    {
        void Receive(DomainEvent message);
    }
}