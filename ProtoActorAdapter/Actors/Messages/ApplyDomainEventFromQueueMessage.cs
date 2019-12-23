using Domain;

namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class ApplyDomainEventFromQueueMessage
    {
        public DomainEvent DomainEvent { get; }

        public ApplyDomainEventFromQueueMessage(DomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}