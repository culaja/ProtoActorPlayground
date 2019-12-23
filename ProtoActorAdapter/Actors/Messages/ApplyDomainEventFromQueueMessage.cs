using Domain;

namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class ApplyDomainEventFromQueueMessage
    {
        public IDomainEvent DomainEvent { get; }

        public ApplyDomainEventFromQueueMessage(IDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }

        public override string ToString() => $"{nameof(ApplyDomainEventFromQueueMessage)}: Event number: {DomainEvent.Number}.";
    }
}