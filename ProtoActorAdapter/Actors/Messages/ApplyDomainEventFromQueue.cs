using Domain;

namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class ApplyDomainEventFromQueue
    {
        public DomainEvent DomainEvent { get; }

        public ApplyDomainEventFromQueue(DomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}