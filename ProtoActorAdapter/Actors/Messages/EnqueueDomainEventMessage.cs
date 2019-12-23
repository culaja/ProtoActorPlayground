using Domain;

namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class EnqueueDomainEventMessage
    {
        public DomainEvent DomainEvent { get; }

        public EnqueueDomainEventMessage(DomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
        
        public override string ToString() => $"{nameof(EnqueueDomainEventMessage)}: Event number: {DomainEvent.Number}.";
    }
}