using Domain;

namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class EnqueueDomainEventMessage
    {
        public IDomainEvent DomainEvent { get; }

        public EnqueueDomainEventMessage(IDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
        
        public override string ToString() => $"{nameof(EnqueueDomainEventMessage)}: Event number: {DomainEvent.Number}.";
    }
}