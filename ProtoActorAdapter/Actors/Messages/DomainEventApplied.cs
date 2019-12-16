using Domain;

namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class DomainEventApplied
    {
        public DomainEvent DomainEvent { get; }

        public DomainEventApplied(DomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}