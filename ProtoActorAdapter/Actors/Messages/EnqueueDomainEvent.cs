using Domain;

namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class EnqueueDomainEvent
    {
        public DomainEvent Event { get; }

        public EnqueueDomainEvent(DomainEvent @event)
        {
            Event = @event;
        }
    }
}