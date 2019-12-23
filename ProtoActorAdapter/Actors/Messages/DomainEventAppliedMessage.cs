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
        
        public override string ToString() => $"{nameof(DomainEventAppliedMessage)}: Event number: {DomainEvent.Number}.";
    }
}