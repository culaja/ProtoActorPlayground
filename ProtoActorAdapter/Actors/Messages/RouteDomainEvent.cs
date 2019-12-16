using Domain;

namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class RouteDomainEvent
    {
        public DomainEvent Event { get; }

        public RouteDomainEvent(DomainEvent message)
        {
            Event = message;
        }

        public string ChildActorId() => Event.AggregateId;
        
        public EnqueueDomainEvent ToEnqueueDomainEvent() => new EnqueueDomainEvent(Event);
        
        public DomainEventRouted ToDomainEventRouted() => new DomainEventRouted(Event.Number);
    }
}