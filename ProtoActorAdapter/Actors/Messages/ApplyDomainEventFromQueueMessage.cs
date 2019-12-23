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

        public override string ToString() => $"{nameof(ApplyDomainEventFromQueueMessage)}: Event number: {DomainEvent.Number}.";
    }
}