using Domain;

namespace ProtoActorAdapter.Actors.Messages
{
    internal sealed class RouteDomainEventMessage
    {
        private readonly DomainEvent _event;

        public RouteDomainEventMessage(DomainEvent @event)
        {
            _event = @event;
        }

        public string ChildActorId() => _event.AggregateId;
        
        public EnqueueDomainEventMessage ToEnqueueDomainEvent() => new EnqueueDomainEventMessage(_event);
    }
}