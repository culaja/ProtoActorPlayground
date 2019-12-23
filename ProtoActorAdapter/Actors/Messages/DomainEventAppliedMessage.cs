using Domain;

namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class DomainEventAppliedMessage
    {
        public DomainEvent DomainEvent { get; }

        public DomainEventAppliedMessage(DomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}