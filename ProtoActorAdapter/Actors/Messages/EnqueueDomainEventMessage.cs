using Domain;

namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class EnqueueDomainEventMessage
    {
        public DomainEvent Event { get; }

        public EnqueueDomainEventMessage(DomainEvent @event)
        {
            Event = @event;
        }
    }
}