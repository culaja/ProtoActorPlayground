using Domain;

namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class DomainEventAppliedMessage
    {
        public IDomainEvent DomainEvent { get; }

        public DomainEventAppliedMessage(IDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
        
        public override string ToString() => $"{nameof(DomainEventAppliedMessage)}: Event number: {DomainEvent.Position}.";
    }
}